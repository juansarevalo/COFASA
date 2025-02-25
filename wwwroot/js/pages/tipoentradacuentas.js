'use strict';

overrideIds({
    tableId: 'tipoEntradaCuentasDt',
    formId: 'tipoEntradaCuentasFormBody',
    addButtonId: 'tipoEntradaCuentasAddBtn',
    gridButtonsId: 'tipoEntradaCuentasGridButtons',
    formValuesId: 'tipoEntradaCuentasForm',
    btnSaveId: 'tipoEntradaCuentasBtnSave',
    btnSaveTextId: 'tipoEntradaCuentasFormBtnSaveText',
})

API.tableURL = '/TipoEntradaCuentas/GetAll?CodCia={CodCia}';
API.saveOneURL = '/TipoEntradaCuentas/SaveOrUpdate';
API.getOneURL = '/TipoEntradaCuentas/GetOne?Id={Id}';

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ overrideShowForm(); });
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': API.tableURL.replace('{CodCia}', $('#codCia').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    { 'data': 'Id' },
                    { 'data': 'TipoEntrada'},
                    { 'data': 'CodContable'},
                    { 'data': 'TipoCuenta' },
                    { 'data': 'FormaCalculo' },
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            return gridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });
        },
        function () {
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data.Id); }
            });
        }
    );
}

function overrideShowForm(id) {
    const isEditing = !(typeof (id) === 'undefined' || id === 0);
    var title = '';

    if (isEditing) {
        title = 'Editar registro';
    } else {
        title = 'Agregar registro';
    }

    dialog = bootbox.dialog({
        title: title,
        message: $('#tipoEntradaCuentasFormBody').val(),
        className: 'modalLarge',
        onEscape: true
    });

    $('#CodCia').val($('#codCia').val());

    initSelect2Paginated(
        'CodContable',
        '/DmgCuentas/GetToSelect2CofasaCatalogo',
        'Cuentas...'
    );

    if (isEditing) {
        $('#Id').val(id);
        $(`#${initValues.formAddButtonTextId}`).text('Editar');

        overrideLoadOne(id);
    }

    overrideFormValidation();
}

function overrideFormValidation() {
    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) { validator.defaultShowErrors(); },
        submitHandlerCb: function (form, event) { doSaveTipoEntCta({}); }
    });
}

function doSaveTipoEntCta({ success, error }) {
    const columna1 = $('#Columna1').val();
    const operacion = $('#Operacion').val();
    const columna2 = $('#Columna2').val();

    const formaCalculo = `${columna1} ${operacion} ${columna2}`;

    let formData = $(`#${initValues.formID}`).serialize();

    formData = formData + `&formaCalculo=${encodeURIComponent(formaCalculo)}`;
    completeDoSave({ success, error, formData });
}

function overrideLoadOne(id) {
    const newURL = API.getOneURL
        .replaceAll('{Id}', id);

    loadOne({
        url: newURL,
        success: function (data) {
            if(data.success){ setDataToForm(data.data); }
        }
    });
}

function setDataToForm(data) {
    if (!isDefined(data)) return;

    if (isDefined(data.TipoEntrada)) $('#TipoEntrada').val(data.TipoEntrada);
    if (isDefined(data.CodContable)) $('#CodContable').val(data.CodContable);
    if (isDefined(data.TipoCuenta)) $('#TipoCuenta').val(data.TipoCuenta);
    if (isDefined(data.FormaCalculo)) $('#FormaCalculo').val(data.FormaCalculo);
}