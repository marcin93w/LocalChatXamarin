var express = require('express');
var path = require('path');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');

var app = express();
var http = require('http').createServer(app);
http.listen(process.env.OPENSHIFT_NODEJS_PORT ||1338, process.env.OPENSHIFT_NODEJS_IP || '127.0.0.1');

require('es6-promise').polyfill();

var mongoose = require('mongoose');
mongoose.Promise = global.Promise;
var mongoUrl = process.env.OPENSHIFT_MONGODB_DB_URL;
if (mongoUrl) {
    mongoUrl += 'lc';
} else {
    mongoUrl = 'mongodb://localhost:27017/localConnect';
    //mongoUrl = 'mongodb://admin:3nPpy4lk7E9S@localhost:27017/lc';
}
mongoose.connect(mongoUrl);

var passport = require('passport');

var api = require('./routes/api');
var chat = require('./services/chat')(http);

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(passport.initialize());

app.use('/api', api);

app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

module.exports = app;
