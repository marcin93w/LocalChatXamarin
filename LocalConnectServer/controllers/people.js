(function (peopleCtrl) {

    var _ = require('underscore');
    var q = require("q");
    var mongoose = require('mongoose');

    var Message = require('../models/message');
    var User = require('../models/user');
    var locationJammer = require('../services/locationJammer');
    
    function mapPeopleCollection(people) {
        return _.map(people, function (person) {
            var newPerson = {
                id: person.id,
                firstname: person.firstname,
                surname: person.surname,
                shortDescription: person.shortDescription,
                avatar: person.avatar,
                unreadMessages: person.unreadMessages
            };
            if (Array.isArray(person.jammedLocation)) {
                newPerson.location = {
                    lon: person.jammedLocation[0],
                    lat: person.jammedLocation[1],
                    disruption: person.settings && person.settings.locationDisruption
                };
            }
            return newPerson;
        });
    }
    
    function getPeopleWithUnreadedMessages(me) {
        var defer = q.defer();
        
        var rules = [
            { 'receiver': mongoose.Types.ObjectId(me.id) },
            { 'status': { $lt: 3 } }];
        Message.aggregate([{
            $match: { $and: rules }
        }, {
            $group: {
                _id: '$sender',
                count: { $sum: 1 }
            }
        }])
        .exec(function (err, unreadMsgs) {
            if (err)
                defer.reject(err);
            else {
                User.populate(unreadMsgs, { path: '_id', select: 'id firstname surname shortDescription settings jammedLocation avatar' },
                function(err, unreadMsgsWithPeople) {
                    if (err)
                        defer.reject(err);
                    else {
                        var people = _.map(unreadMsgsWithPeople, function(msg) {
                            msg._id.unreadMessages = msg.count;
                            return msg._id;
                        });
                        defer.resolve(people);
                    }
                });
            }
        });
        
        return defer.promise;
    }
    
    function getNearestPeople(me) {
        return User.find({
                location: {
                    $near: {
                        $geometry: { type: "Point", coordinates: me.location },
                        $minDistance: 0,
                        $maxDistance: 10000000
                    }
                }
            },
            'id firstname surname shortDescription settings jammedLocation avatar')
        .where("_id").ne(me.id)
        .limit(me.peopleDisplayCount)
        .exec()
        .then(function(people) {
                return people;
            });
    }
    
    peopleCtrl.getNearestPeople = function (req, res) {
        var me = req.user;
        var people = [];
        
        getPeopleWithUnreadedMessages(me)
        .then(function (peopleWithMsgs) {
            people = peopleWithMsgs;
            return getNearestPeople(me, req.user);
        })
        .then(function (nearestPeople) {
            people = _.union(people, _.filter(nearestPeople, function(p) {
                return ! _.find(people, function(person) {
                     return person._id.id === p._id.id;
                });
            }));
            res.json(mapPeopleCollection(people));
        })
        .catch(function(err) {
            res.send(err);
        });
    };

    peopleCtrl.getPerson = function(req, res) {
        User.findOne({ _id: req.params.id }, 'id firstname surname shortDescription settings jammedLocation avatar')
            .exec()
            .then(function(person) {
                res.json(mapPeopleCollection([person])[0]);
            })
            .catch(res.send.bind(res));
    };

    peopleCtrl.getPersonDetails = function(req, res) {
        User.findOne({ _id: req.params.id }, 
            'longDescription',
            function (err, personDetails) {
                if (err) res.send(err);
                res.json(personDetails);
            }
        );
    }
    
    function makeSureThatUserNotExists(name) {
        return new Promise(function(resolve, reject) {
            User.findOne({ username: name })
            .then(function(user) {
                if (user != null) {
                    reject({ "errorCode": 1, "errorMsg": "User already exsists" });
                } else {
                    resolve();
                }
            }, function(error) {
                reject(error);
            });
        });
    }

    peopleCtrl.postRegisterUser = function (req, res) {
        var user = new User({
            username: req.body.Username,
            firstname: req.body.Person.FirstName,
            surname: req.body.Person.Surname,
            shortDescription: req.body.Person.ShortDescription,
            longDescription: req.body.Person.LongDescription,
            settings: {
                locationDisruption: 0,
                peopleDisplayCount: 20
            },
            locationJammerSettings: {
                xDisruption: 0,
                yDisruption: 0
            }
        });
        
        makeSureThatUserNotExists(user.username)
        .then(function () {
            return user.save();
        })
        .then(function() {
            return user.savePassword(req.body.Password);
        })
        .then(function() {
            return user.generateNewToken();
        })
        .then(function (token) {
            res.json({
                registered: true,
                sessionInfo: {
                    token: token,
                    userId: user.id
                }
            });
        })
        .catch(function (err) {
            res.json({
                registered: false,
                errorMsg: err.errorMsg || err,
                errorCode: err.errorCode
            });
        });
    };

    peopleCtrl.getMe = function(req, res) {
        res.json(_.pick(req.user, 'id', 'firstname', 'surname', 'shortDescription', 'avatar'));
    }
    
    peopleCtrl.updateMe = function (req, res) {
        if (!req.body.FirstName || !req.body.Surname) {
            res.send(400);
            return;
        }
        User.findOne({ _id: req.user.id })
            .then(function (user) {
                user.firstname = req.body.FirstName;
                user.surname = req.body.Surname;
                user.shortDescription = req.body.ShortDescription;
                user.longDescription = req.body.LongDescription;
                user.save(function () {
                    res.send(200);
                });
            })
            .catch(function (err) {
                res.send(err);
            });
    }
    
    peopleCtrl.getMySettings = function (req, res) {
        res.json(req.user.settings);
    }
    
    peopleCtrl.updateMySettings = function (req, res) {
        if (req.body.LocationDisruption === undefined || req.body.PeopleDisplayCount === undefined) {
            res.send(400);
            return;
        }
        User.findOne({ _id: req.user.id })
            .then(function (user) {
                user.settings.peopleDisplayCount = req.body.PeopleDisplayCount;
                user.settings.locationDisruption = req.body.LocationDisruption;
                user.locationJammerSettings = locationJammer.generateDisruptionSettings(req.body.LocationDisruption);
            
                var jammingResult = locationJammer.calculateJammedLocationAndNewDisruptionSettings(
                    { Lon: user.location[0], Lat: user.location[1] }, user.settings.locationDisruption, user.locationJammerSettings);

                user.jammedLocation = [jammingResult.location.Lon, jammingResult.location.Lat];
                user.save(function () {
                    console.log('Location updated for user ' + user.name + '(' + user._id + ') at ' + (new Date()));
                    res.send(200);
                });
                user.save(function () {
                    res.send(200);
                });
            })
            .catch(function (err) {
                res.send(500, err);
            });
    }

    peopleCtrl.updateMyLocation = function(req, res) {
        if (isNaN(req.body.Lon) || isNaN(req.body.Lat)) {
            res.send(400);
            return;
        }
        User.findOne({ _id: req.user.id })
            .then(function (user) {
                user.location = [req.body.Lon, req.body.Lat];

                if (user.settings.locationDisruption > 0) {
                    var jammingResult = locationJammer.calculateJammedLocationAndNewDisruptionSettings(
                        req.body, user.settings.locationDisruption, user.locationJammerSettings);

                    user.locationJammerSettings = jammingResult.newDisruptionSettings;
                    user.jammedLocation = [jammingResult.location.Lon, jammingResult.location.Lat];
                } else {
                    user.jammedLocation = user.location;
                }
            
                user.save(function () {
                    console.log('Location updated for user ' + user.name + '(' + user._id + ') at ' + (new Date()));
                    res.send(200);
                });
            })
            .catch(function(err) {
                res.send(500, err);
            });
    }

    peopleCtrl.checkToken = function (personId, token) {
        var defer = q.defer();
        User.findOne({ _id: personId })
            .then(function(user) {
                user.verifyToken(token, function(err, res) {
                    if (err) defer.reject(err);
                    if (res) defer.resolve();
                    else defer.reject('unauthorized');
                });
            });

        return defer.promise;
    }

})(module.exports);