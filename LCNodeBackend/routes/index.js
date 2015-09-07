var express = require('express');
var router = express.Router();

var authController = require('../controllers/user');
var personController = require('../controllers/person');

/* GET home page. */
router.get('/', function (req, res) {
    var data = { a: "asd" };
    res.render('index', { title: JSON.stringify(data) });
});

router.get('/chatTester', function (req, res) {
    res.render('chat.html');
});

router.get('/register', authController.postNewUser);
router.post('/listUsers', authController.getAllUsers);

router.post('/people/add', personController.postNewPerson);
router.get('/people/testGet', personController.getAllPeople);

module.exports = router;