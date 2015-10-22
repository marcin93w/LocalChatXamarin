(function (auth) {
    var _ = require('underscore');

    var config = require('../config');

    var passport = require('passport');
    var BearerStrategy = require('passport-http-bearer').Strategy;
    var BasicStrategy = require('passport-http').BasicStrategy;
    var FacebookTokenStrategy = require('passport-facebook-token');
    
    var User = require('../models/user');
    var Person = require('../models/person');
    
    passport.serializeUser(function (user, done) {
        done(null, user);
    });
    
    passport.deserializeUser(function (user, done) {
        done(null, user);
    });

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
    
    passport.use(new FacebookTokenStrategy({
        clientID: config.FACEBOOK_APP_ID,
        clientSecret: config.FACEBOOK_APP_SECRET
    }, function (accessToken, refreshToken, profile, done) {
        User.findOne({ facebookId: profile.id }, function (error, user) {
            if (user) {
                return done(error, user);
            } else {
                user = new User({ facebookId: profile.id });
                var person = new Person({
                    user: user,
                    firstname: profile.name.givenName,
                    surname: profile.name.familyName,
                    shortDescription: 'Facebook user',
                    avatar: _.first(profile.photos) && _.first(profile.photos).value
            });

                user.save()
                .then(function () {
                    return person.save();
                })
                .then(function() {
                    return done(null, user);
                })
                .catch(function(err) {
                    return done(err, false);
                });
            }
        });
    }));

    auth.checkCredentials = passport.authenticate('basic', { session : false });
    
    auth.isAuthenticated = passport.authenticate('bearer', { session : false });

    auth.facebookAuthenticate = passport.authenticate('facebook-token');

    auth.getUserData = function (req, res) {
        var user = req.user;
        user.generateNewToken()
            .then(function() {
                return Person.findOne({ user: user }, 'id');
            })
            .then(function(person) {
                res.json({
                    token: user.token,
                    personId: person.id
                });
            }, function (error) {
                res.send(error);
            });
    };

})(module.exports);