(function (messagesCtrl) {

    var q = require("q");

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

})(module.exports);