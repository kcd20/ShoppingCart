window.onload = function () { //Only after DOM is completed
    /* setup event listeners for tasks selection */
    let elems = document.getElementsByClassName("add-to-cart");
    for (let i = 0; i < elems.length; i++) {
        elems[i].addEventListener('click', OnAddCartClick);
    }
}

function OnAddCartClick(event) {
    let target = event.currentTarget;

    AddToCart(target.id)
}

function AddToCart(itemId) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Cart/AddToCart");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status != 200) {
                return;
            }

            let data = JSON.parse(this.responseText);
            if (data.status == "success") {
                window.location.href = "/Gallery/AllProducts";
            }
        }
    }

    let itemtocart = {
        ItemId: itemId
    };
    xhr.send(JSON.stringify(itemtocart));

}