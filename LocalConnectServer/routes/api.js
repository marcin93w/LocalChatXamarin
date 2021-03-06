﻿var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');
var peopleController = require('../controllers/people');
var messagesCtrl = require('../controllers/messages');

router.route('/').get(authController.isAuthenticated, function (req, res) {
    res.json({ welcomeMsg: 'Welcome to Local Connect!' });
});

router.route('/register/').post(peopleController.postRegisterUser);

router.route('/login').get(authController.checkCredentials, 
    authController.getUserData);

router.route('/loginWithToken').get(authController.isAuthenticated, 
    authController.getUserData);

router.route('/loginWithFacebook').get(authController.facebookAuthenticate, 
    authController.getUserData);

router.route('/people').get(authController.isAuthenticated, 
    peopleController.getNearestPeople);

router.route('/people/:id').get(authController.isAuthenticated, 
    peopleController.getPerson);

router.route('/personDetails/:id').get(authController.isAuthenticated, 
    peopleController.getPersonDetails);

router.route('/lastMessagesWith/:personId').get(authController.isAuthenticated, 
    messagesCtrl.getLastMessagesWith);

router.route('/me').get(authController.isAuthenticated, 
    peopleController.getMe);

router.route('/me/update').post(authController.isAuthenticated, 
    peopleController.updateMe);

router.route('/me/settings').get(authController.isAuthenticated, 
    peopleController.getMySettings);

router.route('/me/settings/update').post(authController.isAuthenticated, 
    peopleController.updateMySettings);

router.route('/me/updateLocation').post(authController.isAuthenticated, 
    peopleController.updateMyLocation);

module.exports = router;