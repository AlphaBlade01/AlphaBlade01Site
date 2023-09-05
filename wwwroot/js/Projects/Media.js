const thumbnail_input = document.getElementById("Thumbnail");
const previews_input = document.getElementById("Previews");
const thumbnail_preview = document.getElementById("thumbnail-preview");
const previews_container = document.getElementById("previews-container");
const meta_img = previews_container.getElementsByClassName("meta")[0];

thumbnail_input.onchange = (event) => {
    const reader = new FileReader();
    reader.onload = () => thumbnail_preview.src = reader.result;
    reader.readAsDataURL(event.target.files[0]);
};

previews_input.onchange = (event) => {
    while (meta_img.nextSibling) {
        meta_img.nextSibling.remove();
    }

    setTimeout(() => {
        for (const file of event.target.files) {
            const reader = new FileReader();
            const img_element = meta_img.cloneNode();

            img_element.className = "preview"
            previews_container.appendChild(img_element);

            reader.onload = () => img_element.src = reader.result;
            reader.readAsDataURL(file);
        }
    }, 2000);
}