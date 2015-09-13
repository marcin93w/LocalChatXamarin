var express = require('express');
var router = express.Router();

router.get('/', function (req, res) {
    var data = { a: "asd" };
    res.render('index', { title: JSON.stringify(data) });
});

router.get('/chatTester', function (req, res) {
    res.render('chat.html', { user: req.param('user') });
});

module.exports = router;