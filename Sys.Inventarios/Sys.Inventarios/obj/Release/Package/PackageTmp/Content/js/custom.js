var validAjax;
var formAjax = $('.formAjax');
$("button.btnAjax").on('click', function (e) {
    e.preventDefault();
    validAjax = formAjax.validate({
        //== Validate only visible fields
        ignore: ":hidden",
        //== Display error  
        invalidHandler: function (event, validAjax) {
            swal({
                "title": "",
                "text": "Los datos capturados no son correctos.",
                "type": "error",
                "confirmButtonClass": "btn btn-secondary m-btn m-btn--wide"
            });
        },
        //== Submit valid form
        submitHandler: function (form) {
        }
    });
    if (validAjax.form() == false) {
        $("#userSubs-error").hide();
        return;
    }
    $(".loadingAjax").show();
    formAjax.ajaxSubmit({
        success: function (response) {
            if (response.IsSuccess == true) {
                if (response.Id == -1) {
                    window.location.href = response.Message;
                } else {
                    swal({
                        "title": "",
                        "text": response.Message,
                        "type": "success",
                        "confirmButtonClass": "btn btn-secondary m-btn m-btn--wide"
                    }).then(function () {
                        $(formAjax).find("input[type=text]").val("");
                        $(formAjax).find("select").val("");
                        $(".ocultarDespues").modal();
                        //$("#IdMod").val(response.Id);
                        //window.location = urlGeneral + "tramites";
                    });
                }
            }
            $(".loadingAjax").hide();
        },
        error: function (request, status, error) {
            swal({
                "title": "",
                "text": "No se puede conectar al servidor, intentelo más tarde!" + request.responseText,
                "type": "error",
                "confirmButtonClass": "btn btn-secondary m-btn m-btn--wide"
            }).then(function () {
                //window.location = urlGeneral + "tramites";
            });
            $(".loadingAjax").hide();
        }
    });
});