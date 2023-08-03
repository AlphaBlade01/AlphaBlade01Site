var commentsBtn = document.getElementById("comments-btn");
var commentsFrame = document.getElementById("comments-menu");
var commentForm = document.getElementById("comment-form");
var commentsContainer = document.getElementById("comments-container");


function calculateDeleteButtonSize() {
    commentsContainer.childNodes.forEach(comment => {
        if (comment.className != "comment") return;

        let delete_btn = comment.querySelector(".delete-btn");
        let parent_height = comment.clientHeight;

        delete_btn.style.width = `${parent_height - 10}px`;
    });
}


function handleHttpResponse(response) {
    if (response.ok) {
        localStorage.setItem("openComments", true);
        location.reload();
    }
}


Array.from(document.getElementsByClassName("delete-btn")).forEach(deleteButton => {
    deleteButton.addEventListener("click", () => {
        let id = deleteButton.parentNode.getAttribute("comment-id");
        fetch(`/comment/delete?id=${id}`, {
            method: "POST"
        }).then(handleHttpResponse)
            .catch(console.error);
    });
});


// Open comments frame if reloaded after a comment submission
if (localStorage.getItem("openComments") === "true") {
    localStorage.setItem("openComments", false);
    commentsFrame.style.transform = "translateX(0%)";
}


// Toggle comments frame
commentsBtn.addEventListener("click", () => {
    let transform = commentsFrame.style.transform != "" ? commentsFrame.style.transform : "0";
    let transform_value = parseInt(transform.match(/(-?\d+)/g)[0]);
    let new_transform = transform_value ^ 100;

    commentsFrame.style.transform = `translateX(${new_transform}%)`;
});


// Override default form submission in order to publish comment
commentForm.addEventListener("submit", (e) => {
    e.preventDefault();

    const formData = new FormData(commentForm);

    fetch(commentForm.action, {
        method: "POST",
        body: formData
    }).then(handleHttpResponse)
        .catch(console.error);
});

window.onload = calculateDeleteButtonSize;
window.onresize = calculateDeleteButtonSize;