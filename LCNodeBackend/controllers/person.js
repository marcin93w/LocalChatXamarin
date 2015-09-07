(function (personCtrl) {

    var _ = require('underscore');

    var Person = require('../models/person');
    
    personCtrl.postNewPerson = function (req, res) {
        var person = new Person({
            firstname: req.body.firstname,
            surname: req.body.surname,
            shortDescription: req.body.shortDescription,
            longDescription: req.body.longDescription
        });
        
        person.save(function (err) {
            if (err)
                res.send(err);
            
            res.json({ message: 'Person successfully registered!' });
        });
    };
    
    personCtrl.getAllPeople = function (req, res) {
        Person.find({}, 'firstname surname shortDescription', function (err, people) {
            if (err)
                res.send(err);

            res.json(people);
        });
    };

})(module.exports);