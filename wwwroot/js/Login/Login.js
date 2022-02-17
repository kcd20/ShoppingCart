function validateForm() {
    var x = document.forms["loginForm"]["username"].value;
    var y = document.forms["loginForm"]["password"].value;
    var flag = false;
    if ((x == null || x == "") && (y == null || y == "")) {
        alert("Username and password cannot be empty at the same time");
    } else if (x == null || x == "") {
        alert("Username can not be empty");
    } else if (y == null || y == "") {
        alert("Password can not be empty");
    } else {
        flag = true;
    }
    return flag;
}