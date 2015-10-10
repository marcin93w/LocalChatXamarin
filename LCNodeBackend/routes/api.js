﻿var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');
var peopleController = require('../controllers/people');
var messagesCtrl = require('../controllers/messages');

router.route('/').get(authController.isAuthenticated, function (req, res) {
    res.json({ welcomeMsg: 'Welcome to Local Connect!' });
});

router.route('/login').get(authController.checkCredentials, 
    authController.getUserData);

router.route('/loginWithToken').get(authController.isAuthenticated, 
    authController.getUserData);

router.route('/people').get(authController.isAuthenticated, 
    peopleController.getAllPeople);

router.route('/personDetails/:id').get(authController.isAuthenticated, 
    peopleController.getPersonDetails);

router.route('/lastMessagesWith/:personId').get(authController.isAuthenticated, 
    messagesCtrl.getLastMessagesWith);

router.route('/register/').post(authController.isAuthenticated, 
    peopleController.postRegisterUser);


//router.route('/me').get(authController.isAuthenticated, 
//    peopleController.getMe);

//router.route('/me/name').get(authController.isAuthenticated, 
//    peopleController.getMyName);

module.exports = router;