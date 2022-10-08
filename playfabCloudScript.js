handlers.UpdateUser = function (args, context) {
    var playfabid = args.playfabSentID;
    var dataPayload = {};
     dataPayload[args.guid] = args.jsonToSend;
      var updateUserDataResult = server.UpdateUserData({
         PlayFabId: playfabid,
         Data: dataPayload,
         Permission : "Public"
     });
 };