var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');
var userController = require('../controllers/user');
var personController = require('../controllers/person');

router.route('/').get(authController.isAuthenticated, function (req, res) {
    res.json({ welcomeMsg: 'Welcome to Local Connect!' });
});

router.route('/login').get(authController.checkCredentials, 
    userController.getAuthToken);

router.route('/people').get(authController.isAuthenticated, 
    personController.getAllPeople);

module.exports = router;