let jwtToken = null;
let username = null;
let connection = null;

// -------------------- helper --------------------
function getAntiForgeryToken() {
    const inp = document.querySelector('input[name="__RequestVerificationToken"]');
    return inp ? inp.value : null;
}

// -------------------- login --------------------
async function doLogin() {
    const userInput = document.getElementById("loginUser");
    const passInput = document.getElementById("loginPassword");

    const userName = userInput?.value.trim();
    const password = passInput?.value.trim();
    if (!userName || !password) { alert("Enter username and password"); return; }

    try {
        const formData = new FormData();
        formData.append("UserName", userName);
        formData.append("Password", password);
        const af = getAntiForgeryToken();
        if (af) formData.append("__RequestVerificationToken", af);

        const res = await fetch("/Auth/Login", {
            method: "POST",
            body: formData,
            credentials: "same-origin"
        });

        if (!res.ok) {
            const text = await res.text();
            console.error("Login failed HTTP", res.status, text);
            alert("Login failed: " + (text || res.status));
            return;
        }

        const data = await res.json();
        if (!data || !data.token) {
            alert("Token not found. Inspect server response.");
            return;
        }

        sessionStorage.setItem("AuthToken", data.token);
        sessionStorage.setItem("Username", data.username || userName);

        // Redirect user to chat page — start SignalR on the Chat page itself on load
        window.location.href = "/Chat/Index";
    } catch (err) {
        console.error("Login error:", err);
        alert("An unexpected error occurred during login.");
    }
}


// -------------------- SignalR setup (call this from Chat/Index DOMContentLoaded) --------------------

function startSignalRWithToken() {
    const token = sessionStorage.getItem("AuthToken");
    if (!token) { console.warn("No token — cannot start SignalR"); return; }

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub", { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveMessage", (sender, message, createdAt) => {
        const box = document.getElementById("chatBox");
        if (!box) return;
        const m = document.createElement("div");
        m.textContent = `${sender}: ${message} (${new Date(createdAt).toLocaleTimeString()})`;
        box.appendChild(m);
        box.scrollTop = box.scrollHeight;
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(e => console.error("SignalR start error:", e));
}

async function sendMessage() {
    const sender = sessionStorage.getItem("Username");
    const recipient = document.getElementById("recipient")?.value?.trim();
    const message = document.getElementById("message")?.value?.trim();

    if (!recipient || !message) {
        alert("Recipient and message are required!");
        return;
    }

    const token = sessionStorage.getItem("AuthToken");
    if (!token) {
        alert("You are not logged in!");
        return;
    }

    const formData = new FormData();
    formData.append("Sender", sender ?? "");
    formData.append("Recipient", recipient);
    formData.append("Message", message);

    const response = await fetch("/Chat/Send", {
        method: "POST",
        body: formData,
        headers: {
            //  this line is required
            "Authorization": "Bearer " + token
        }
    });

    if (!response.ok) {
        const err = await response.text();
        console.error("Send failed:", response.status, err);
        alert("Send failed: " + response.status);
        return;
    }

    console.log("Message sent!");
    document.getElementById("message").value = "";
}




// -------------------- DOM wiring --------------------
document.addEventListener("DOMContentLoaded", () => {
    // Wire send form if present
    const sendForm = document.getElementById("sendForm");
    if (sendForm) {
        sendForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            await sendMessage();
        });
    }

    // If this is Chat/Index page, start SignalR using stored token
    const isChatPage = !!document.getElementById("chatBox");
    if (isChatPage) {
        startSignalRWithToken();
    }

    // optional: wire login form if you use a login form with id="loginForm"
    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", (e) => { e.preventDefault(); doLogin(); });
    }
});


