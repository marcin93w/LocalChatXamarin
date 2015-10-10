(function (peopleCtrl) {

    var Person = require('../models/person');
    var User = require('../models/user');

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
            username: req.body.username,
            password: req.body.password
        });
        var person = new Person({
            user: user,
            firstname: req.body.firstname,
            surname: req.body.surname,
            shortDescription: req.body.shortDescription,
            longDescription: req.body.longDescription
        });
        
        makeSureThatUserNotExists(user.username)
        .then(function () {
            return user.save();
        })
        .then(function() {
            return person.save();
        })
        .then(function () {
            res.json({
                registered: true,
                token: user.token,
                personId: person.id
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

    //peopleCtrl.getMe = function(req, res) {
    //    Person.find({ user: req.user }, 
    //        'firstname surname shortDescription longDescription',
    //        function(err, me) {
    //            if (err) res.send(err);
    //            res.json(me);
    //        });
    //}

    //peopleCtrl.getMyName = function (req, res) {
    //    Person.findOne({ user: req.user }, 
    //        'firstname surname',
    //        function (err, me) {
    //        if (err) res.send(err);
    //        res.json(me);
    //    });
    //}

})(module.exports);