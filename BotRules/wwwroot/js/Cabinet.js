$.ajaxSetup({
    headers: {
        "RequestVerificationToken":
            $('[name="__RequestVerificationToken"]').val()
    }
});

document.getElementById("CreateNewTarget").onclick = function () {
    document.getElementById("createNewTarget").style.display = "table";
    document.getElementById("creating").style.display = "block";
    document.getElementById("CreateNewTarget").style.display = "none";
};

Array.from(document.getElementsByClassName("TurnBot")).forEach(function (element) {
    element.onclick = function () {        
        var value = element.value;
        var data = JSON.stringify({ "BotId": element.id, "TurnOn": ( value == "Выключить" ? 0 : 1) });
        $.ajax({
            type: "POST",
            url: "/bot/turnbot",
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (servdata) {
                if (servdata == true)                    
                    element.value = (value == "Выключить" ? "Включить" : "Выключить");
            }
        });
    }
});

document.getElementById("BeginCreating").onclick = function () {
    var botName = $("#BotName").val();
    var botKey = $("#BotKey").val();
    var data = JSON.stringify({ "Name": botName, "key": botKey });
    $.ajax({
        type: "POST",
        url: "/bot/createbot",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (servdata) {
            if (servdata == true)
                location.reload();
            else
                alert(servdata);
        }
    });
}