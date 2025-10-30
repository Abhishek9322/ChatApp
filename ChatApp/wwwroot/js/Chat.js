const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub") // must match the hub route in Program.cs
    .build();

connection.on("ReceiveMessage", (user, message, time) => {
    const chatBox = document.getElementById("chatBox");
    const div = document.createElement("div");
    div.innerHTML = `<b>${user}</b>: ${message} (${new Date(time).toLocaleTimeString()})`;
    chatBox.appendChild(div);
});

connection.start()
    .then(() => console.log("✅ Connected to SignalR"))
    .catch(err => console.error("❌ SignalR Error:", err));
