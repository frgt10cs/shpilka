var lastFunctionNumber = 0;
var commandsArray = [];
var lastOpenedFunction;
var trueFunctionsList = ["2"];
var comboboxVariants = [
    {
        type: 0,
        name: "Выберите дейтсвие",
        arguments: []
    },
    {
        type: 1,
        name: "Добавить в хр-е",
        arguments: ["Ответ", "Имя хранилища"]
    },
    {
        type: 2,
        name: "Вывести данные",
        arguments: ["Имя хранилища"]
    }

];
var emptyFunction = {
    "command": null,
    "inputFunctions": [
        {
            "arguments": [],
            "argumentsName": [],
            "functionType": 0,
            "isAuto": false
        }
    ],
    "response": {
        "answer": null,
        "photo": null
    }
};

function comboboxChanger() {
    boxes = $(".selectFunction");
    boxes.change(function () {
        var block = $(this).parent().parent().parent().find(".argumentsConatiner");
        newType = $(this).val();
        block.empty();
        row = "";
        for (var i = 0; i < comboboxVariants[newType].arguments.length; i++) {
            row = row + "<div class=\"row\">" + "<div class=\"rowTitle rtif\">" + comboboxVariants[newType].arguments[i] + ":</div>" + "<input type=\"text\" class=\"rowValue " + i + "\" value=\"\"> </div> "
        }
        block.append(row);
    });
}

function comboboxGenerator() {
    res = "<select class=\"selectFunction\">";
    comboboxVariants.forEach(function (element) {
        res = res + "<option value=\"" + element.type + "\">" + element.name + "</option>";
    });
    res = res + "</select>";
    return res;
}

function comboboxGenerator2(defaultVal) {
    defaultVal = +defaultVal;
    res = "<select class=\"selectFunction\">";

    if (defaultVal === undefined) {
        var i = 0;
        comboboxVariants.forEach(function (element) {
            res = res + "<option value=\"" + element.type + "\">" + element.name + "</option>";
        });
    } else {
        for (var i = 0; i < defaultVal; i++) {
            res = res + "<option value=\"" + comboboxVariants[i].type + "\">" + comboboxVariants[i].name + "</option>";
        }
        res = res + "<option selected = \"selecte   d\" value=\"" + comboboxVariants[defaultVal].type + "\">" + comboboxVariants[defaultVal].name + "</option>"
        for (var i = defaultVal + 1; i < comboboxVariants.length; i++) {
            res = res + "<option value=\"" + comboboxVariants[i].type + "\">" + comboboxVariants[i].name + "</option>";
        }
    }
    res = res + "</select>";
    return (res);
};

function setDeleteFunction() {
    var functionIcons = $(".delFunctionIcon");
    functionIcons.off();
    functionIcons.on("click", function () {
        localId = $(this).parent().attr('id');
        $(this).parent().remove();
        commandsArray[localId] = null;
    });
}

function setOpenCloseFunction() {

    var details = $(".details");
    var closeDetails = $("#closeDetails");

    //КНОПКА ВЫВОДА ДЕТАЛЕЙ
    details.off()
    details.on("click", function () {

        lastOpenedFunction = $(this).parent().attr('id');

        inputSubFuncs = commandsArray[lastOpenedFunction].inputFunctions;

        inputSubFuncs.forEach(function (element) {

            fType = element.functionType;
            $("#inputFunctions").append("<div class=\"inputFunctionContainer\"><div class=\"arrow\">⇩</div><div class=\"inputFunction\"><div class=\"delFunctionIcon delOpt\" title=\"Удалить функцию\">X</div><div class= \"row\" ><div class=\"rowTitle rtif\">Действие:" + comboboxGenerator2(fType) + "</div></div > <div class = \"argumentsConatiner\"> </div> </div>");

            row = ""
            if (fType != 0) {

                for (var i = 0; i < comboboxVariants[fType].arguments.length; i++) {
                    row = row + "<div class=\"row\">" + "<div class=\"rowTitle rtif\">" + comboboxVariants[fType].arguments[i] + ":</div>" + "<input type=\"text\" class=\"rowValue 0\" value=\"" + element.arguments[i] + "\">" + "</div> ";
                }
                $(".argumentsConatiner:last").append(row);
            }
        }
        );

        comboboxChanger();

        $(".darkness").css({ display: "block" });
        $(".functionOption").css({ display: "block" });

        $(".functionOption").prepend("<div class=\"commandFunction\"><div class= \"row\" ><div class=\"rowTitle\">Команда:</div><input type=\"text\" class=\"rowValue 0\"></div><div class=\"row\"><div class=\"rowTitle\">Ответ:</div><input type=\"text\" class=\"rowValue 1\" /></div></div >");
        $(".functionOption").children(".commandFunction").find(".rowValue.0").val($(this).parent().find(".rowValue.0").val());
        $(".functionOption").children(".commandFunction").find(".rowValue.1").val($(this).parent().find(".rowValue.1").val());

        var functionIcons = $(".delOpt");
        functionIcons.off();
        functionIcons.on("click", function () {
            $(this).parent().parent().remove();
        });
    });

    //КНОПКА СКРЫТИЯ ДЕТАЛЕЙ
    closeDetails.off();
    closeDetails.on("click", function () {

        cmdFunction = $(".functionOption").children(".commandFunction");

        //ЗАМЕНА ЗНАЧЕНИЯ ОСНОВНОЙ ФУНКЦИИ
        newCommand = cmdFunction.find(".0").val();
        newAnswer = cmdFunction.find(".1").val();

        $("#" + lastOpenedFunction).find(".0").val(newCommand);
        $("#" + lastOpenedFunction).find(".1").val(newAnswer);

        $(".darkness").css({ display: "none" });
        $(".functionOption").css({ display: "none" });

        //ЗАМЕНА ЗНАЧЕНИЙ ПОДФУНКЦИЙ

        //НОВЫЙ command
        commandNew = cmdFunction.find(".rowValue.0").val();

        //НОВЫЙ inputFunctions
        inputFunctionsNew = [];
        containers = $(".argumentsConatiner");

        containers.each(function () {
            fType = $(this).parent().find(".selectFunction").val();
            argumentsNameNew = [];
            argumentsNew = [];

            for (var i = 0; i < comboboxVariants[fType].arguments.length; i++) {
                argumentsNameNew.push(comboboxVariants[fType].arguments[i]);
            };

            $(this).find(".rowValue").each(function () {
                argumentsNew.push($(this).val());
            });

            if (fType != 0) {                
                if (argumentsNew.indexOf('') == -1) {
                    newSubFunction = {
                        "argumentsName": argumentsNameNew,
                        "arguments": argumentsNew,
                        "isAuto": false,
                        "functionType": fType
                    };
                    inputFunctionsNew.push(newSubFunction);
                }
            };

        });

        //НОВЫЙ response
        responseNew = {
            "answer": cmdFunction.find(".rowValue.1").val(),
            "photo": null
        };

        //СБОРКА ВСЕГО И ПРИСВОЕНИЕ
        change = {
            "command": commandNew,
            "inputFunctions": inputFunctionsNew,
            "response": responseNew
        };

        commandsArray[lastOpenedFunction] = change
        $(".functionOption").children(".commandFunction").remove();
        $('#inputFunctions').empty();
        $(".functionOption").css({ display: "none" });
    });

};





$(".functionOption").css({ display: "none" });

$.ajaxSetup({
    headers: {
        "RequestVerificationToken":
            $('[name="__RequestVerificationToken"]').val()
    }
});

$.ajax({
    type: "POST",
    url: "/bot/getfunctional",
    data: {
        "botId": document.getElementById("BotId").value
    },
    dataType: "json",
    success: function (servdata) {
        if (servdata != false) {
            servdata.commandFunctions.forEach(function (element) {
                $("#commandFunctions").append("<div class=\"commandFunction\" id = \"" + lastFunctionNumber + "\" > <div class=\"delFunctionIcon\" title=\"Удалить функцию\">X</div><div class= \"row\" ><div class=\"rowTitle\">Команда:</div><input type=\"text\" class=\"rowValue 0\" value=\"" + element.command + "\"></div><div class=\"row\"><div class=\"rowTitle\">Ответ:</div><input type=\"text\" class=\"rowValue 1\" value=\"" + element.response.answer + "\" /></div><input type=\"button\" value=\"Подробнее\" class=\"details\"/></div >");
                commandsArray.push(element);
                lastFunctionNumber++;
            });
            setDeleteFunction();
            setOpenCloseFunction();
        }
    }
});

document.getElementById("addFunctionIcon").onclick = function () {
    $("#commandFunctions").append("<div class=\"commandFunction\" id = \"" + lastFunctionNumber + "\" > <div class=\"delFunctionIcon\" title=\"Удалить функцию\">X</div><div class= \"row\" ><div class=\"rowTitle\">Команда:</div><input type=\"text\" class=\"rowValue 0\"></div><div class=\"row\"><div class=\"rowTitle\">Ответ:</div><input type=\"text\" class=\"rowValue 1\"/></div><input type=\"button\" value=\"Подробнее\" class=\"details\"/></div >");
    commandsArray.push(emptyFunction);

    setDeleteFunction();
    setOpenCloseFunction();
    lastFunctionNumber++;
};
document.getElementById("addOptFunctionIcon").onclick = function () {
    $("#inputFunctions").append("<div class=\"inputFunctionContainer\"><div class=\"arrow\">⇩</div><div class=\"inputFunction\"><div class=\"delFunctionIcon delOpt\" title=\"Удалить функцию\">X</div><div class= \"row\" ><div class=\"rowTitle rtif\">Действие:" + comboboxGenerator() + "</div></div > <div class = \"argumentsConatiner\"> </div> </div>");

    comboboxChanger();

    var functionIcons = $(".delOpt");
    functionIcons.off();
    functionIcons.on("click", function () {
        $(this).parent().parent().remove();
    });
};
document.getElementById("saveButton").onclick = function () {

    //Лечим баг с изменением основной функции НЕ через "подробнее"
    $(".commandFunction").each(function () {
        commandsArray[$(this).attr('id')].command = $(this).find(".rowValue.0").val()
        commandsArray[$(this).attr('id')].response.answer = $(this).find(".rowValue.1").val()

    });

    //Исключение плохих функций

    //Исключение удалённых функций
    commandsArraySorted = [];
    commandsArray.forEach(function (element) {        
        if (element !== null) {
            commandsArraySorted.push(element);
        }
    });

    //Исключение функций с пустыми полями
    if (commandsArraySorted.length != 0) {

        commandsArraySorted = [];
        commandsArray.forEach(function (element) {
            if (element !== null) {
                if (element.length != 0) {
                    if ((element.command != '') && (element.response.answer != '')) {
                        commandsArraySorted.push(element);
                    };
                };
            };
        });
    };

    //Исключение функций с пустыми подфункцииями
    commandsArraySorted.forEach(function (element) {
        if (element.inputFunctions.length != 0) {
            newInputFunctions = [];
            if (element.inputFunctions[0].argumentsName.length == 0) {
                element.inputFunctions = [];
            };
        };
    }); 

    //Исправление isAuto
    commandsArraySorted.forEach(function (element) {

        element.inputFunctions.forEach(function (subElement) {

            if (trueFunctionsList.indexOf(subElement.functionType) != -1) {
                subElement.isAuto = true;
            } else {
                subElement.isAuto = false;
            }
        });

    });

    res = [];
    commandsArraySorted.forEach(function (element) {
        isGood = true;
        element.inputFunctions.forEach(function (subElement) {
            if (subElement.arguments.indexOf('') != -1) {
                isGood = false;
            }
        });

        if (isGood) {
            res.push(element);
        }
    });
    commandsArraySorted = res;

    //Исправление isAuto
    commandsArraySorted.forEach(function (element) {

        element.inputFunctions.forEach(function (subElement) {

            if (trueFunctionsList.indexOf(subElement.functionType) != -1) {
                subElement.isAuto = true;
            } else {
                subElement.isAuto = false;
            }
        });

    });

    //Отправка json'а
    var mail = JSON.stringify(
        {
            "BotId": $("#BotId").val(),
            "commandFunctions": commandsArraySorted
        });

    $.ajax({
        type: "POST",
        url: "/bot/SaveFunctional",
        data: mail,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function () {
        }
    });
};

var filesPanel = document.getElementById("files");
var storagesPanel = document.getElementById("storages");
var functionsPanel = document.getElementById("commandFunctions");

document.getElementById("functionsButton").onclick = function () {
    functionsPanel.style.display = "block";
    filesPanel.style.display = "none";
    document.getElementById("addNewStorage").style.display = "none";
    storagesPanel.style.display = "none";
    $("#UploadStorageToServer").css("display", "none");
    $(".file_upload").css("display", "none");
};

document.getElementById("storagesButton").onclick = function () {
    document.getElementById("addNewStorage").style.display = "block";
    functionsPanel.style.display = "none";
    filesPanel.style.display = "none";
    storagesPanel.style.display = "block";
    $("#UploadStorageToServer").css("display", "block");
    $(".file_upload").css("display", "block");
    $.ajax({
        url: "/editor/getstorages",
        type: "POST",
        dataType: "json",
        success: function (storagesName) {
            var storages = $("#storages");
            storages.empty();
            $.each(storagesName, function (index, storage) {
                storages.append("<div class=\"targetInfo tis\"><div class= \"topDiv\" ><img class=\"targetPhoto\" src=\"/images/Storage.png\" /><div class=\"targetTitle\">" + storage + "</div></div ><input type=\"button\" class=\"targetEdit TurnBot showStorageData\" value=\"Просмотр\" id=\"" + storage + "\" /><input type=\"button\" class=\"targetEdit TurnBot removeStorage\" value=\"Удалить\" id=\"" + storage + "\" /></div >");
            });            
            storages.append("<div class=\"bContainer\" id=\"dataTable\"><div id = \"closeData\" class= \"close\" >X</div ><div class=\"dataStorageContainer\"></div></div >");
            setAction();
        }
    });
};

document.getElementById("filesButton").onclick = function () {
    functionsPanel.style.display = "none";
    filesPanel.style.display = "block";
    storagesPanel.style.display = "none";
    $("#UploadStorageToServer").css("display", "none");
    $(".file_upload").css("display", "none");
};

document.getElementById("showCreateNewStorage").onclick = function () {
    $(".targetInfo").css("height", "unset");
    $("#createNewStorage").css("display", "block");
    $("#creating").css("display", "block");
    $(this).css("display", "none");
};

document.getElementById("addFieldStorage").onclick = function () {
    $("#names").append("<b class= \"cbt\" >Название хр-х данных</b ><input type=\"text\" class=\"inputData columnName\" id=\"BotKey\" />");
};

document.getElementById("createStorage").onclick = function () {
    var answers = [];
    $(".columnName").each(function () {
        if ($(this)!="")
            answers.push(($(this).val()));
    });
    var storage = JSON.stringify(
        {
            "Name": $("#StorageName").val(),
            "ColumnNames": answers
        });    
    $.ajax({
        url: "/editor/createstorage",
        type: "post",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: storage,
        success: function (data) {
            if (data == true) {
                location.reload();
            }
            else {
                alert(data);
            }
        }
    });
    closeCreate();
};

document.getElementById("addNewStorage").onclick = function () {
    $(".darkness").css("display", "block");
    $("#addingContainer").css("display", "block");
    $("#closeCreate").css("display", "block");
    $("#createNewTarget").css("display", "flex");
};

function setAction() {
    $(".showStorageData").on("click", function () {
        $(".darkness").css("display", "block");
        $("#dataTable").css("display", "block");
        $.ajax({
            type: "POST",
            url: "/editor/getstoragedata",
            data: { "storageName": this.id },
            dataType: "json",
            success: function (data) {
                if (data === null || data === undefined || data=="") {
                    $(".dataStorageContainer").append("<p>Пусто</p>");
                }
                else {
                    var dataSt = "";
                    var preData = data[0].split(";");
                    var first = "";
                    var st = "";
                    $.each(preData.slice(1), function (index, td) {
                        if(td!="")
                            st += "<th>" + td + "</th>";
                    });
                    first = "<tr class=\"first\">" + st + "</tr>";
                    $.each(data.slice(1), function (index, string) {
                        var str = "";
                        $.each((string + "").split(";").slice(1), function (index, column) {
                            str += "<td>" + column + "</td>";
                        });
                        dataSt += "<tr>" + str + "</tr>";
                    });
                    $(".dataStorageContainer").append("<table>" + first + dataSt + "</table>");
                }                
            }
        });
    });

    $(".removeStorage").on("click", function () {
        var but = this.parentElement;
        $.ajax({
            url: "/editor/removestorage",
            type: "POST",
            dataType: "json",
            data: {
                "storageName": this.id
            },
            success: function (data) {                
                if (data == true)
                    but.remove();
                else
                    alert("Не удалось удалить файл");
            }
        });
    });
    document.getElementById("closeData").onclick = function () {
        $(".darkness").css("display", "none");
        $("#dataTable").css("display", "none");
        $(".dataStorageContainer").empty();
    };
}


function closeCreate() {
    $(".darkness").css("display", "none");
    $("#createNewTarget").css("display", "none");
    $("#closeCreate").css("display", "none");
}
document.getElementById("closeCreate").onclick = closeCreate;
