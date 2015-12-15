/// <reference path="~/scripts/jquery-2.1.4.min.js" />
/// <reference path="~/scripts/jquery.signalR-2.2.0.min.js" />

'use strict';

$(function () {
    var currentUserName = prompt('Choose a user name for yourself:');
    var inUse = true;
    
    while (inUse === true)
    {
        $.ajax({
            url: '/home/verifyusernameinuse',
            data: { userName: currentUserName },
            async: false
        }).done(function (data, textStatus, jqXHR) {
            inUse = data.InUse;

            if (inUse)
                currentUserName = prompt('Please choose another user name, this one is already in use.');
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        });
    }

    var zigChatHubProxy = $.connection.zigChatHub;

    zigChatHubProxy.client.updateChat = function (userName, message, isPm) {
        var $newMessage;

        if (currentUserName === userName) {
            $newMessage = $('<div class="panel panel-primary" style="margin-left: 7em; background-color: #337ab7;">' +
                                '<div style="padding: .5em; color: white; text-align: right;">' + message + '</div>' +
                           '</div>');
        } else {
            $newMessage = $('<div class="panel panel-primary" style="margin-right: 7em; background-color: ' + (isPm ? 'green' : '#337ab7') + ';">' +
                                '<div style="padding: .5em; color: white; border-bottom: .1em solid white; font-size: 11px;">' + userName + '</div>' +
                                '<div style="padding: .5em; color: white;">' + message + '</div>' +
                            '</div>');
        }

        $('#chat').append($newMessage);

        $newMessage[0].scrollIntoView(true);
    };

    zigChatHubProxy.client.updateUsersOnline = function (data) {
        if (!data.Success) {
            alert(data.ErrorMessage);
            return;
        }

        var $users = $('#users');
        $users.html(null);

        for (var user of data.UsersOnline) {
            if (user === currentUserName)
                $users.append($('<p class="user current">' + user + '</p>'));
            else {
                var $user = $('<p class="user">' + user + '</p>');
                $user.click(function () {
                    var $message = $('#message');
                    $message.val('@' + $(this).text() + ' ');
                    $message.focus();
                });

                $users.append($user);
            }
        }
    };

    $.connection.hub.start()
        .done(function () {
            var status = zigChatHubProxy.server.connectUser(currentUserName).done(function (data, textStatus, jqXHR) {
                if (!data.Success) {
                    alert(data.ErrorMessage);
                    return;
                }

                var $message = $('#message');

                var sendMessage = function () {
                    $message.focus();

                    if ($message.val() === '')
                        return;

                    zigChatHubProxy.server.sendMessage(currentUserName, $message.val());
                    $message.val('');
                };

                $message.keyup(function (data) {
                    if (data.which === 13)
                        sendMessage();
                });

                $('#send').click(sendMessage);

                $message.focus();
            });
        })
        .fail(function () {
            console.log('Could not connect.');
        });
});