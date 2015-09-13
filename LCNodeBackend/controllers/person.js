(function (personCtrl) {

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

    function makeLocationAnObject(collection) {
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
    
    personCtrl.getAllPeople = function (req, res) {
        Person.find({}, 'firstname surname shortDescription location')
        .where("user").ne(req.user)
        .lean()
        .exec(function (err, people) {
            if (err) res.send(err);
            res.json(makeLocationAnObject(people));
        });
    };

    personCtrl.getMe = function(req, res) {
        Person.find({ user: req.user }, 
            'firstname surname shortDescription longDescription',
            function(err, me) {
                if (err) res.send(err);
                res.json(me);
            });
    }

    personCtrl.getMyName = function (req, res) {
        Person.findOne({ user: req.user }, 
            'firstname surname',
            function (err, me) {
            if (err) res.send(err);
            res.json(me);
        });
    }

})(module.exports);