(function (messagesCtrl) {

    var q = require("q");

    var ObjectId = require('mongoose').Types.ObjectId; 
    var Message = require('../models/message');
    var Person = require('../models/person');

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
        return Person
            .findOne({ user: me }, 'id')
            .then(function (person) {
                var criteria = {
                    $or: [
                        { sender: person.id, receiver: new ObjectId(personId) },
                        { receiver: person.id, sender: new ObjectId(personId) }
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
            });
    }

    messagesCtrl.getLastMessagesWith = function(req, res) {
        var olderThen = req.query.olderThen;
        if (!olderThen) {
            getMessagesWith(req.user, req.params.personId)
                .then(res.json.bind(res), res.send.bind(res));
        } else {
            Message.findOne({ '_id': olderThen }, 'dateTime')
            .then(function(msg) {
                return getMessagesWith(req.user, req.params.personId, msg.dateTime)
                    .then(res.json.bind(res));
            })
            .catch(res.send.bind(res));
        }
    };

})(module.exports);