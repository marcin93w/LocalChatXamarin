(function (peopleCtrl) {

    var _ = require('underscore');
    var q = require("q");
    var mongoose = require('mongoose');

    var Person = require('../models/person');
    var Message = require('../models/message');
    var User = require('../models/user');
    var locationJammer = ('../services/locationJammer');
    
    function beautifyPeopleCollection(people) {
        return _.map(people, function (person) {
            var newPerson = {
                _id: person.id,
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
                    disruption: person.locationDisruption
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
                Person.populate(unreadMsgs, { path: '_id', select: 'id firstname surname shortDescription locationDisruption jammedLocation avatar' },
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
    
    function getNearestPeople(me, user) {
        return Person.find({
                location: {
                    $near: {
                        $geometry: { type: "Point", coordinates: me.location },
                        $minDistance: 0,
                        $maxDistance: 10000000
                    }
                }
            },
            'id firstname surname shortDescription locationDisruption jammedLocation avatar')
        .where("user").ne(user)
        .limit(20)
        .exec()
        .then(function(people) {
                return people;
            });
    }
    
    peopleCtrl.getNearestPeople = function (req, res) {
        var me = null;
        var people = [];
        Person.findOne({ user: req.user }, 'location')
        .then(function(myPerson) {
            me = myPerson;
            return getPeopleWithUnreadedMessages(me);
        })
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
            res.json(beautifyPeopleCollection(people));
        })
        .catch(function(err) {
            res.send(err);
        });
    };

    peopleCtrl.getPerson = function(req, res) {
        Person.findOne({ _id: req.params.id }, 'id firstname surname shortDescription locationDisruption jammedLocation avatar')
            .exec()
            .then(res.json.bind(res))
            .catch(res.send.bind(res));
    };

    peopleCtrl.getPersonDetails = function(req, res) {
        Person.findOne({ _id: req.params.id }, 
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
            password: req.body.Password
        });
        var person = new Person({
            user: user,
            firstname: req.body.Person.FirstName,
            surname: req.body.Person.Surname,
            shortDescription: req.body.Person.ShortDescription,
            longDescription: req.body.Person.LongDescription
        });
        
        makeSureThatUserNotExists(user.username)
        .then(function () {
            return user.save();
        })
        .then(function() {
            return person.save();
        })
        .then(function() {
            return user.generateNewToken();
        })
        .then(function () {
            res.json({
                registered: true,
                sessionInfo: {
                    token: user.token,
                    personId: person.id
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
        Person.findOne({ user: req.user }, 
            'id firstname surname shortDescription avatar',
            function(err, me) {
                if (err) res.send(err);
                res.json(me);
            });
    }
    
    peopleCtrl.updateMe = function (req, res) {
        if (!req.body.FirstName || !req.body.Surname || !req.body.ShortDescription || !req.body.LongDescription) {
            res.send(400);
            return;
        }
        Person.findOne({ user: req.user })
            .then(function (person) {
                person.firstname = req.body.FirstName;
                person.surname = req.body.Surname;
                person.shortDescription = req.body.ShortDescription;
                person.longDescription = req.body.LongDescription;
                person.save(function () {
                    res.send(200);
                });
            })
            .catch(function (err) {
                res.send(err);
            });
    }
    
    peopleCtrl.getMySettings = function (req, res) {
        Person.findOne({ user: req.user }, 
            'id locationDisruption',
            function (err, me) {
            if (err) res.send(err);
            res.json(me);
        });
    }
    
    peopleCtrl.updateMySettings = function (req, res) {
        if (!req.body.locationDisruption) {
            res.send(400);
            return;
        }
        Person.findOne({ user: req.user })
            .then(function (person) {
                person.locationDisruption = req.body.locationDisruption;
                person.locationJammerSettings = locationJammer.generateDisruptionSettings(req.body.locationDisruption);
                person.save(function () {
                    res.send(200);
                });
            })
            .catch(function (err) {
                res.send(err);
            });
    }

    peopleCtrl.updateMyLocation = function(req, res) {
        if (isNaN(req.body.Lon) || isNaN(req.body.Lat)) {
            res.send(400);
            return;
        }
        Person.findOne({ user: req.user })
            .then(function (person) {
                var jammingResult = locationJammer.calculateJammedLocationAndNewDisruptionSettings(
                    person.location, person.locationDisruption, person.locationJammerSettings);

                person.locationJammerSettings = jammingResult.newDisruptionSettings;
                person.location = [req.body.Lon, req.body.Lat];
                person.jammedLocation = [jammingResult.location.Lon, jammingResult.location.Lat];
                person.save(function () {
                    console.log('Location updated for user ' + person.name + '(' + person._id + ') at ' + (new Date()));
                    res.send(200);
                });
            })
            .catch(function(err) {
                res.send(err);
            });
    }

})(module.exports);