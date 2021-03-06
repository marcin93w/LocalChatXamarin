﻿(function (messagesCtrl) {

    var q = require("q");

    var ObjectId = require('mongoose').Types.ObjectId; 
    var Message = require('../models/message');

    var messagesFetchCount = 10;

    messagesCtrl.saveMessage = function (message) {
        var saveResultPromise = q.defer();

        message.save(function (err) {
            if (err) {
                console.log('Message could not be saved in database (' + JSON.stringify(message) + ')' + err);
                saveResultPromise.reject(err);
            }

            saveResultPromise.resolve(message.id);
        });

        return saveResultPromise.promise;
    };
    
    messagesCtrl.setDisplayed = function(msgId) {
        var saveResultPromise = q.defer();

        Message.findOne({ _id: msgId }, 'status')
            .then(function(message) {
                message.status = 3;
                return message.save()
                    .then(function() {
                        saveResultPromise.resolve();
                    });
            })
            .catch(function(err) {
                saveResultPromise.reject(err);
            });
        
        return saveResultPromise.promise;
    }
    
    function getMessagesWith(me, personId, toDate) {
        var criteria = {
            $or: [
                { sender: me.id, receiver: new ObjectId(personId) },
                { receiver: me.id, sender: new ObjectId(personId) }
            ]
        };
        if (toDate) {
            criteria.dateTime = { $lt: toDate };
        }
        return Message
            .find(criteria, 'id sender text dateTime status')
            .sort('-dateTime')
            .limit(messagesFetchCount)
            .exec();
    }

    messagesCtrl.getLastMessagesWith = function(req, res) {
        var olderThan = req.query.olderThan;
        if (!olderThan) {
            getMessagesWith(req.user, req.params.personId)
                .then(res.json.bind(res), res.send.bind(res));
        } else {
            Message.findOne({ '_id': olderThan }, 'dateTime')
            .then(function(msg) {
                return getMessagesWith(req.user, req.params.personId, msg.dateTime)
                    .then(res.json.bind(res));
            })
            .catch(res.send.bind(res));
        }
    };

    messagesCtrl.findSender = function(msgId) {
        return Message.findOne({ _id: msgId }, 'sender');
    };

})(module.exports);