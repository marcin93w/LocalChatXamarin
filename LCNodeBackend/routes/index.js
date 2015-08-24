var express = require('express');
var router = express.Router();

var authController = require('../controllers/authentication');

/* GET home page. */
router.get('/', function (req, res) {
    var data = { a: "asd" };
    res.render('index', { title: JSON.stringify(data) });
});

router.get('/chatTester', function (req, res) {
    res.render('chat.html');
});

router.get('/register', authController.postNewUser);
router.get('/listUsers', authController.getAllUsers);

module.exports = router;