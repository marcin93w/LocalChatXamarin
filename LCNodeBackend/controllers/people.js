﻿(function (peopleCtrl) {

    var _ = require('underscore');
    var mongoose = require('mongoose');

    var Person = require('../models/person');
    var Message = require('../models/message');
    var User = require('../models/user');

    function beautifyPeopleCollection(people, unreadMsgs) {
        return _.map(people, function(person) {
            var newPerson = {
                _id: person.id,
                firstname: person.firstname,
                surname: person.surname,
                shortDescription: person.shortDescription,
                avatar: person.avatar
            };
            if (Array.isArray(person.location)) {
                newPerson.location = {
                    lon: person.location[0],
                    lat: person.location[1]
                };
            }
            var personId = mongoose.Types.ObjectId(person.id);
            var personMsgs = _.find(unreadMsgs, function(msg) {
                return msg._id.id === personId.id;
            });
            if (personMsgs)
                newPerson.unreadMessages = personMsgs.count;
            return newPerson;
        });
    }    
    
    peopleCtrl.getAllPeople = function (req, res) {
        Person.find({}, 'id firstname surname shortDescription location avatar')
        .where("user").ne(req.user)
        .lean()
        .exec(function (err, people) {
            if (err) res.send(err);
            res.json(beautifyPeopleCollection(people));
        });
    };
    
    peopleCtrl.getNearestPeople = function (req, res) {
        Person.findOne({ user: req.user }, 'location')
        .then(function(me) {
            return Person.find({location: {
                        $near : {
                            $geometry: { type: "Point", coordinates: me.location },
                            $minDistance: 0,
                            $maxDistance: 10000000
                        }
                    }}, 
                    'id firstname surname shortDescription location avatar')
                .where("user").ne(req.user)
                .limit(20)
                .exec()
                .then(function (people) {

                    var peopleIds = _.map(people, function(p) { return mongoose.Types.ObjectId(p.id); });
                    var rules = [
                        { 'receiver': mongoose.Types.ObjectId(me.id) },
                        { 'status': { $lt: 3 } }, 
                        { 'sender': { $in : peopleIds } }];
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
                                res.send(err);
                            else {
                                res.json(beautifyPeopleCollection(people, unreadMsgs));
                            }
                        });
                });
        })
        .catch(function(err) {
            res.send(err);
        });
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

    peopleCtrl.updateMyLocation = function(req, res) {
        if (isNaN(req.body.Lon) || isNaN(req.body.Lat)) {
            res.send(400);
            return;
        }
        Person.findOne({ user: req.user })
            .then(function(person) {
                person.location = [req.body.Lon, req.body.Lat];
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