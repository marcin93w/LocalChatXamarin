(function (peopleCtrl) {

    var _ = require('underscore');

    var Person = require('../models/person');

    function beautifyPeopleCollection(collection) {
        collection.forEach(function(element) {
            if (Array.isArray(element.location)) {
                element.location = {
                    'lon': element.location[0],
                    'lat': element.location[1]
                };
            }
        });
        return collection;
    }    
    
    peopleCtrl.getAllPeople = function (req, res) {
        Person.find({}, 'id firstname surname shortDescription location')
        .where("user").ne(req.user)
        .lean()
        .exec(function (err, people) {
            if (err) res.send(err);
            res.json(beautifyPeopleCollection(people));
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

    peopleCtrl.getMe = function(req, res) {
        Person.find({ user: req.user }, 
            'firstname surname shortDescription longDescription',
            function(err, me) {
                if (err) res.send(err);
                res.json(me);
            });
    }

    peopleCtrl.getMyName = function (req, res) {
        Person.findOne({ user: req.user }, 
            'firstname surname',
            function (err, me) {
            if (err) res.send(err);
            res.json(me);
        });
    }

})(module.exports);