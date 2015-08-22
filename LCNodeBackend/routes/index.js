var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
    res.render('index', { title: 'Express' });
});

router.get('/chatTester', function (req, res) {
    res.render('chat.html');
});

module.exports = router;