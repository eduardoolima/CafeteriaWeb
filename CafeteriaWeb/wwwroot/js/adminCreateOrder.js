function submitForm() {
    document.getElementById("productsValidation").innerHTML = ""
    document.getElementById("clientValidation").innerHTML = ""
    document.getElementById("adressValidation").innerHTML = ""
    var products = [];
    let amountItensValidation = false
    $('input[name="itemId"]').each(function (index) {
        var itemId = $(this).val();

        var itemAmount = $('input[name="itemAmount"]').eq(index).val();
        var itemPrice = $('input[name="itemPrice"]').eq(index).val();
        debugger
        var parsedAmount = parseFloat(itemAmount)
        if (parsedAmount > 0) {
            amountItensValidation = true;
        }
        var product = {
            productId: itemId,
            amount: itemAmount,
            price: itemPrice
        };
        products.push(product);
    });
    debugger
    if (!amountItensValidation) {
        document.getElementById("productsValidation").innerHTML = "Selecione pelo menos um item"
        return
    }

    var observation = $('#observation').val();
    var isPaid = $('#isPayed').prop('checked');
    var forDelivery = $('#forDelivery').prop('checked');
    var externalClient = $('#externalClient').prop('checked');
    var userId = $('#selectClient').val();
    var addressId = $('#selectAdress').val();

    if (!externalClient && userId == 0) {
        document.getElementById("clientValidation").innerHTML = "Selecione um Cliente"
        return
    }
    if (forDelivery && addressId == 0) {
        document.getElementById("adressValidation").innerHTML = "Selecione um Endereço"
        return
    }
    var data = {
        Products: products,
        Order: {
            Observation: observation,
            IsPaid: isPaid,
            ForDelivery: forDelivery,
            UserId: userId,
            AdressId: addressId
        },
        ExternalClient: externalClient,
    };
    var jsonData = JSON.stringify(data)

    $.ajax({
        type: 'POST',
        url: '/Admin/AdminOrder/CreateOrder/',
        data: { jsonData: jsonData },
        success: function (response, statusText, xhr) {
            if (xhr.status === 200) {
                window.location.href = '/Admin/AdminOrder/Index/';
            } else {
                console.log('Status de resposta não é 200. Status:', xhr.status);
                console.log(response)
            }            
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}


$(document).ready(function () {
    function updateVisibilit() {
        var externalClientChecked = $('#externalClient').is(':checked');
        var forDeliveryChecked = $('#forDelivery').is(':checked');

        if (externalClientChecked && forDeliveryChecked) {
            $('#client').hide();
            $('#adress').hide();
        } else if (forDeliveryChecked) {
            $('#client').show();
            $('#adress').show();
        } else {
            $('#adress').hide();
            if (!externalClientChecked) {
                $('#client').show();
            } else {
                $('#client').hide();
            }
        }
    }

    $('#externalClient').change(function () {
        updateVisibilit();
    });

    $('#forDelivery').change(function () {
        updateVisibilit();
    });

    updateVisibilit();

    $('#selectClient').change(function () {
        let selectClient = $('#selectClient')
        $.ajax({
            type: "POST",
            url: "/Admin/AdminOrder/ListAdress/",
            data: { userId: selectClient.val() },
            success: function (data) {
                AddAdresses(data)
            },
            error: function (error) {
                if (error.status == 401) {
                    return;
                }
            }
        });

    });

    function AddAdresses(adresses) {
        $.each(adresses, function (index, item) {
            $('#selectAdress').append($('<option>', {
                value: item.id,
                text: item.street + ' ' + item.number
            }));
            console.log(item)
        })
    }
});