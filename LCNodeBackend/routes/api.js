var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');
var userController = require('../controllers/user');
var peopleController = require('../controllers/people');

router.route('/').get(authController.isAuthenticated, function (req, res) {
    res.json({ welcomeMsg: 'Welcome to Local Connect!' });
});

router.route('/login').get(authController.checkCredentials, 
    userController.getUserData);

router.route('/loginWithToken').get(authController.isAuthenticated, 
    userController.getUserData);

router.route('/people').get(authController.isAuthenticated, 
    peopleController.getAllPeople);

router.route('/me').get(authController.isAuthenticated, 
    peopleController.getMe);

router.route('/me/name').get(authController.isAuthenticated, 
    peopleController.getMyName);

module.exports = router;