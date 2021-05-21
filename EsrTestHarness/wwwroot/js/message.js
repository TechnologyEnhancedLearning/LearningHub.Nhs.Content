"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

connection.start().then(function () {
    connection.invoke('getConnectionId')
        .then(function (connectionId) {
            $.ajax({
                type: 'post',
                url: '/SignalRHubConnection',
                data: JSON.stringify({ connectionId }),
                contentType: "application/json; charset=utf-8"
            });

        });

}).catch(function (err) {
    console.error(err);
    displayMessage(`<li><h5 class="text-danger">Connection to server unsuccessful.</h5></li>`)
});

connection.onclose(() => {
    displayMessage('<li><h5 class="text-danger">Disconnected from server, please reload the page.</h5></li>')
});

connection.on("ReceiveLmsTraffic", function (request, response) {
    displayMessage(`<li><strong>${timeNow()}<br/></strong><div class="d-flex"><div class="col-6"><strong>Request:</strong><br/>${request}</div><div class="col-6"><strong>Response:</strong><br/>${response}</div></div><hr/></li>`);
});


function timeNow() {
    const options = {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit', second: '2-digit',
        hour12: false
    };

    return new Intl.DateTimeFormat('default', options).format(new Date()).replace(',', '');
}

function displayMessage(message) {
    const $d = $('#messagesList');
    $d.append(message);
    $d[0].scrollIntoView(false);
    $('#clearLog').removeClass('d-none');
}