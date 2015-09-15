(function (user) {
    //var User = require('../models/user');
    
    user.getUserData = function (req, res) {
        var token = 'asda';
        var user = req.user;
        user.token = token;
        user.save();
        res.json({
            token: user.token,
            userId: user._id
        });
    };
    
    //user.postNewUser = function (req, res) {
    //    var user = new User({
    //        username: req.query.username,
    //        password: req.query.password
    //    });
        
    //    user.save(function (err) {
    //        if (err)
    //            res.send(err);
            
    //        res.json({ message: 'User successfully registered!' });
    //    });
    //};
    
    //for DEBUG purposes
    //var Person = require('../models/person');
    //user.getAllUsers = function (req, res) {
    //    User.find(function (err, users) {
    //        if (err)
    //            res.send(err);
            
    //        var person = new Person({
    //            firstname: req.body.firstname,
    //            surname: req.body.surname,
    //            shortDescription: req.body.shortDescription,
    //            longDescription: req.body.longDescription,
    //            user: users[0]._id
    //        });
            
    //        person.save(function (err) {
    //            if (err)
    //                res.send(err);
                
    //            res.json({ message: 'Person successfully registered!' });
    //        });

    //        //res.json(users);
    //    });
    //};

})(module.exports);