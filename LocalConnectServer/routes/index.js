var express = require('express');
var router = express.Router();

router.get('/', function (req, res) {
    var data = { a: "asd" };
    res.render('index', { title: JSON.stringify(data) });
});

router.get('/chatTester', function (req, res) {
    var socketAddres = process.env.OPENSHIFT_NODEJS_IP ? 'wss://lc-fancydesign.rhcloud.com:8443' : 'http://localhost:1338';
    res.render('chat.html', { personId: req.param('personId'), socketIoUrl: socketAddres });
});

module.exports = router;