$(function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .configureLogging(signalR.LogLevel.Information) // Optional: Enable detailed logging
        .build();

    // Start the SignalR connection
    connection.start().then(function () {
        // Join the group only once when the connection is established (e.g., on page load or chat open)
        const groupId = $('#groupId').val(); // Assuming the group ID is stored in a hidden field or similar

        // Only join the group once when the connection is established
        connection.invoke("JoinGroup", groupId).then(() => {
            console.log(`Joined group ${groupId}`);
        }).catch(function (err) {
            console.error(err.toString());
        });

        // Now handle the message sending
        $('#sendButton').click(function () {
            const messageContent = $('#newMessage').val();
            const userName = '@User.Identity.Name'; // Update this based on your authentication system

            // Send the message to SignalR
            connection.invoke("SendMessageToGroup", groupId, userName, messageContent)
                .catch(function (err) {
                    return console.error(err.toString());
                });

            // Clear the message input after sending
            $('#newMessage').val('');
        });
    }).catch(function (err) {
        console.error(err.toString());
    });

    // Listen for messages received from SignalR
    connection.on("ReceiveMessage", function (user, message) {
        $('#chatMessages').append('<div><strong>' + user + ':</strong> ' + message + '</div>');
    });
});
