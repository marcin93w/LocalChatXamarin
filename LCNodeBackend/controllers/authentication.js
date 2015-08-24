(function (auth) {
    var passport = require('passport');
    var BearerStrategy = require('passport-http-bearer').Strategy;
    var BasicStrategy = require('passport-http').BasicStrategy;
    var User = require('../models/user');
    
    passport.use(new BearerStrategy(
        function (token, done) {
            User.findOne({ token: token }, function (err, user) {
                if (err) { return done(err); }
                if (!user) { return done(null, false); }
                return done(null, user, { scope: 'all' });
            });
        }
    ));

    passport.use(new BasicStrategy(
        function (username, password, callback) {
            User.findOne({ username: username }, function (err, user) {
                if (err) { return callback(err); }
                
                // No user found with that username
                if (!user) { return callback(null, false); }
                
                // Make sure the password is correct
                user.verifyPassword(password, function (err, isMatch) {
                    if (err) { return callback(err); }
                    
                    // Password did not match
                    if (!isMatch) { return callback(null, false); }
                    
                    // Success
                    return callback(null, user);
                });
            });
        }
    ));
    
    auth.checkCredentials = passport.authenticate('basic', { session : false });
    
    auth.isAuthenticated = passport.authenticate('bearer', { session : false });

    auth.getAuthToken = function(req, res) {
        var token = 'asda';
        var user = req.user;
        user.token = token;
        user.save();
        res.json({ token: token });
    };

    auth.postNewUser = function (req, res) {
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
    
    auth.getAllUsers = function(req, res) {
        User.find(function(err, users) {
            if (err)
                res.send(err);

            res.json(users);
        });
    };

})(module.exports);