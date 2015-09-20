(function (auth) {
    var passport = require('passport');
    var BearerStrategy = require('passport-http-bearer').Strategy;
    var BasicStrategy = require('passport-http').BasicStrategy;
    
    var User = require('../models/user');
    var Person = require('../models/person');
    
    passport.use(new BearerStrategy(
        function (token, done) {
            User.findOne({ token: token }, function (err, user) {
                if (err) {
                     return done(err);
                }
                if (!user) {
                     return done(null, false);
                }
                return done(null, user, { scope: 'all' });
            });
        }
    ));

    passport.use(new BasicStrategy(
        function (username, password, callback) {
            User.findOne({ username: username }, function (err, user) {
                if (err) {
                     return callback(err);
                }
                if (!user) {
                     return callback(null, false);
                }
                
                user.verifyPassword(password, function (err, isMatch) {
                    if (err) {
                         return callback(err);
                    }
                    if (!isMatch) {
                         return callback(null, false);
                    }
                    
                    return callback(null, user);
                });
            });
        }
    ));
    
    auth.checkCredentials = passport.authenticate('basic', { session : false });
    
    auth.isAuthenticated = passport.authenticate('bearer', { session : false });

    auth.getUserData = function (req, res) {
        var token = 'asda';
        var user = req.user;
        user.token = token;
        user.save()
            .then(function() {
                return Person.findOne({ user: user }, 'id');
            })
            .then(function(personId) {
                res.json({
                    token: user.token,
                    personId: personId
                });
            }, function (error) {
                res.send(error);
            });
    };

})(module.exports);