async function searchUser() {
    const searchBox = document.getElementById("searchUser");
    const keyword = searchBox.value.trim();
    const token = sessionStorage.getItem("AuthToken");

    if (!keyword) {
        alert("Enter a username to search");
        return;
    }

    try {
        const response = await fetch(`/Dash/Search?username=${encodeURIComponent(keyword)}`, {
            headers: { "Authorization": "Bearer " + token }
        });

        if (!response.ok) {
            throw new Error("Search failed");
        }

        const users = await response.json();
        const resultsDiv = document.getElementById("searchResults");
        resultsDiv.innerHTML = "";

        if (users.length === 0) {
            resultsDiv.innerHTML = "<p>No users found.</p>";
            return;
        }

        users.forEach(u => {
            const btn = document.createElement("button");
            btn.className = "btn btn-outline-primary m-1";
            btn.textContent = u.userName;
            btn.onclick = () => {
                document.getElementById("recipient").value = u.userName;
                alert("Recipient selected: " + u.userName);
            };
            resultsDiv.appendChild(btn);
        });
    } catch (err) {
        console.error("❌ Error searching user:", err);
    }
}
