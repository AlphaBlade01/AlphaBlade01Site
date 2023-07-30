let thumbnailSrc = document.getElementById("thumbnail").src;
let overlayDiv = document.createElement("div");
let overlayWrapper = document.createElement("div");
let expandedThumbnail = document.createElement("img");

function initialiseOverlay() {
    overlayDiv.hidden = true;

    expandedThumbnail.src = thumbnailSrc
    expandedThumbnail.setAttribute("class", "expanded-thumbnail");

    overlayWrapper.style = "position: relative; width: 100%; height: 60%; display: flex; justify-content: center; top: 50%; transform: translateY(-50%);";
    overlayWrapper.appendChild(expandedThumbnail);

    overlayDiv.setAttribute("class", "overlay");
    overlayDiv.appendChild(overlayWrapper);
    overlayDiv.addEventListener("click", disableOverlay);

    document.body.appendChild(overlayDiv);
}

function disableOverlay() {
    overlayDiv.hidden = true;
}

function enableOverlay(img) {
    overlayDiv.hidden = false;
}

thumbnail.addEventListener("click", enableOverlay);

initialiseOverlay();