const thumbnail_input = document.getElementById("Thumbnail");
const previews_input = document.getElementById("Previews");
const thumbnail_preview = document.getElementById("thumbnail-preview");
const previews_container = document.getElementById("previews-container");
const meta_img = document.getElementById("meta-img");
const meta_vid = document.getElementById("meta-vid");

thumbnail_input.onchange = (event) => {
    const reader = new FileReader();
    reader.onload = () => thumbnail_preview.src = reader.result;
    reader.readAsDataURL(event.target.files[0]);
};

previews_input.onchange = (event) => {
    previews_container.replaceChildren([]);

    for (const file of event.target.files) {
        const reader = new FileReader();
        const childNode = file["type"].startsWith("image") ? meta_img.cloneNode() : meta_vid.cloneNode();

        childNode.removeAttribute("id");
        childNode.className = "preview"
        previews_container.appendChild(childNode);

        reader.onload = () => childNode.src = reader.result;
        reader.readAsDataURL(file);
    }
}