(function (peopleCtrl) {

    var _ = require('underscore');

    var Person = require('../models/person');
    
    //personCtrl.postNewPerson = function (req, res) {
    //    var person = new Person({
    //        firstname: req.body.firstname,
    //        surname: req.body.surname,
    //        shortDescription: req.body.shortDescription,
    //        longDescription: req.body.longDescription
    //    });
        
    //    person.save(function (err) {
    //        if (err)
    //            res.send(err);
            
    //        res.json({ message: 'Person successfully registered!' });
    //    });
    //};

    function beautifyPeopleCollection(collection) {
        collection.forEach(function(element) {
            if (Array.isArray(element.location)) {
                element.location = {
                    'lon': element.location[0],
                    'lat': element.location[1]
                };
            }
            element.userId = element.user;
            delete element.user;
        });
        return collection;
    }    
    
    peopleCtrl.getAllPeople = function (req, res) {
        Person.find({}, 'firstname surname shortDescription location user')
        .where("user").ne(req.user)
        .lean()
        .exec(function (err, people) {
            if (err) res.send(err);
            res.json(beautifyPeopleCollection(people));
        });
    };

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