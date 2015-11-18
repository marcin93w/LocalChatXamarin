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

    messagesCtrl.getLastMessagesWith = function(req, res) {
        Person
            .findOne({ user: req.user }, 'id')
            .then(function(person) {
                return Message
                    .find({
                            $or: [
                                { sender: person.id, receiver: new ObjectId(req.params.personId) }, 
                                { receiver: person.id, sender: new ObjectId(req.params.personId) }
                            ]
                        }, 
                        'id sender text dateTime status'
                    )
                    .sort('-dateTime')
                    .limit(messagesFetchCount)
                    .exec();
            })
            .then(function (messages) {
                res.json(messages);
            }, function(error) {
                res.send(error);
            });
    };

})(module.exports);