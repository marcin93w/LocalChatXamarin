var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');
var userController = require('../controllers/user');
var personController = require('../controllers/person');

router.route('/').get(authController.isAuthenticated, function (req, res) {
    res.json({ welcomeMsg: 'Welcome to Local Connect!' });
});

router.route('/login').get(authController.checkCredentials, 
    userController.getUserData);

router.route('/loginWithToken').get(authController.isAuthenticated, 
    userController.getUserData);

router.route('/people').get(authController.isAuthenticated, 
    personController.getAllPeople);

router.route('/me').get(authController.isAuthenticated, 
    personController.getMe);

router.route('/me/name').get(authController.isAuthenticated, 
    personController.getMyName);

module.exports = router;