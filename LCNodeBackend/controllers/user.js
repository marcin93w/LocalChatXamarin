(function (user) {
    var User = require('../models/user');
    
    user.getUserData = function (req, res) {
        var token = 'asda';
        var user = req.user;
        user.token = token;
        user.save();
        res.json(user);
    };
    
    user.postNewUser = function (req, res) {
        var user = new User({
            username: req.query.username,
            password: req.query.password
        });
        
        user.save(function (err) {
            if (err)
                res.send(err);
            
            res.json({ message: 'User successfully registered!' });
        });
    };
    
    //for DEBUG purposes
    user.getAllUsers = function (req, res) {
        User.find(function (err, users) {
            if (err)
                res.send(err);
            
            res.json(users);
        });
    };

})(module.exports);