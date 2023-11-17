
function loadProductsData(selectedProducts) {
    
    $('input[type=checkbox]').each(function (index) {
        var itemId = parseInt($(this).val());
        console.log(selectedProducts.includes(itemId))
        if (selectedProducts.includes(itemId)) {
            $(this).prop('checked', true)
        }
    });
}

function getFormData() {
    document.getElementById("productsValidation").innerHTML = ""
    document.getElementById("priceValidation").innerHTML = ""
    document.getElementById("startDateValidation").innerHTML = ""
    document.getElementById("endDateValidation").innerHTML = ""
    var products = [];

    $('input[type=checkbox]:checked').each(function (index) {
        var itemId = $(this).val();
        products.push(itemId);
    });

    if (products.length < 1) {
        document.getElementById("productsValidation").innerHTML = "Selecione pelo menos um item"
        return
    }

    var price = parseFloat($('#price').val());
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();

    if (price <= 0 || isNaN(price)) {
        document.getElementById("priceValidation").innerHTML = "Defina um preço válido"
        return
    }

    if (!validateDates()) {
        return
    }

    var data = {
        Products: products,
        OnSalePrice: price,
        SaleStart: startDate,
        SaleEnd: endDate,
    };
    //var jsonData = JSON.stringify(data)
    return (data)
}
function submitForm() {
    var formData = getFormData()
    jsonData = JSON.stringify(formData)
    $.ajax({
        type: 'POST',
        url: '/Admin/Promotions/CreatePromotion/',
        data: { jsonData: jsonData },
        success: function (response, statusText, xhr) {
            if (xhr.status === 200) {
                window.location.href = '/Admin/Promotions/Index/';
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

function submitFormEdit() {
    debugger
    let promotionId = document.getElementById("promotionId").value
    var formData = getFormData()

    var data = {
        PromotionId: promotionId,
        Products: formData.Products,
        OnSalePrice: formData.OnSalePrice,
        SaleStart: formData.SaleStart,
        SaleEnd: formData.SaleEnd,
    };

    jsonData = JSON.stringify(data)

    $.ajax({
        type: 'POST',
        url: '/Admin/Promotions/EditPromotion/',
        data: { jsonData: jsonData },
        success: function (response, statusText, xhr) {
            if (xhr.status === 200) {
                window.location.href = '/Admin/Promotions/Index/';
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

function validateDates() {

    let startDateValidation = document.getElementById('startDateValidation')
    let endDateValidation = document.getElementById('endDateValidation')

    startDateValidation.innerText = "";
    endDateValidation.innerText = "";

    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;
    var currentDate = new Date();
    if (startDate && endDate) {
        var startDateObj = new Date(startDate);
        var endDateObj = new Date(endDate);

        if (startDateObj < currentDate || endDateObj < currentDate) {
            startDateValidation.innerText = "As datas não podem ser anteriores à data atual.";
            return false;
        }

        if (endDateObj <= startDateObj) {
            endDateValidation.innerText = "A data de término deve ser posterior à data de início.";
            return false; 
        } else {
            endDateValidation.innerText = "";
            return true;
        }
    } else {
        startDateValidation.innerText = "Por favor, preencha ambas as datas";
        return false;
    }
}

$(document).ready(function () {
    document.getElementById('startDate').addEventListener('change', validateDates);
    document.getElementById('endDate').addEventListener('change', validateDates);
});