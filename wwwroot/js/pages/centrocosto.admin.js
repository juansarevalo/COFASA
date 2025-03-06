'use strict';

let ccAccountsGridDialog;
let dialogCopyCC;

overrideIds({
    tableId: 'centroCostoDt',
    formId: 'centroCostoFormBody',
    formValuesId: 'centroCostoForm',
    addButtonId: 'centroCostoAddBtn',
    gridButtonsId: 'centroCostoGridButtons',
    btnSaveId: 'centroCostoFormBtnSave',
    btnSaveTextId: 'centroCostoFormBtnSaveText',
})

API.tableURL = '/CentroCosto/GetAll?ciaCod={codCia}';
API.saveOneURL = '/CentroCosto/SaveOrUpdate';
API.getOneURL = '/CentroCosto/GetOne?codCia={codCia}&codCentroCosto={codCentroCosto}';

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
                    'url': API.tableURL.replace('{codCia}', $('#codCia').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    {'data': 'COD_CIA'},
                    {'data': 'CENTRO_COSTO'},
                    {'data': 'DESCRIPCION'},
                    {
                        'data': 'ACEPTA_DATOS',
                        render: function (data, type, row) {
                            return data==='S' ? 'Sí' : 'No';
                        }
                    },
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
                if (action === 'edit') { overrideShowForm(data.COD_CIA, data.CENTRO_COSTO); }
                if (action === 'accounts') { showCCGrid(data.COD_CIA, data.CENTRO_COSTO, data.DESCRIPCION); }
            });
        }
    );
}

function initCentroCostoInputMask() {
    $('#CENTRO_COSTO').mask('99-999-99');
}

function overrideShowForm(codCia, codCentroCosto) {
    showForm(codCia, function (isEditing) {
        initCentroCostoInputMask();
        
        overrideFormValidation();
        $('#COD_CIA').val($('#codCia').val());

        if (isEditing) {
            $('#isUpdating').val(codCia);
            $(`#${initValues.formAddButtonTextId}`).text('Editar');

            overrideLoadOne(codCia, codCentroCosto);

            $('#CENTRO_COSTO_PADRE_DIV').hide();
        } else {
            initSelect2Paginated(
                'CENTRO_COSTO_PADRE',
                '/CentroCosto/GetToSelect2Father',
                'Centro de Costo Padre');
        }
    });
}

//Se usa en form de cuentas
let codigoCentroCosto;
function showCCGrid(codCia, codCentroCosto, description) {
    $('#codCC').val(codCentroCosto);
    codigoCentroCosto = codCentroCosto;

    ccAccountsGridDialog = bootbox.dialog({
        title: `Cuentas asociadas al centro de costo<br/><small class="text-capitalize text-secondary">${codCentroCosto} - ${description}</small>`,
        message: $(ccAccountsGridHtmlId).val(),
        onEscape: true,
        className: 'modalLarge',
    });

    ccAccountsPrepareButtons();
    ccAccountsInitGrid();
}

function overrideFormValidation() {
    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) { validator.defaultShowErrors(); },
        submitHandlerCb: function (form, event) {
            if ($("#CENTRO_COSTO").val() || $("#CENTRO_COSTO_PADRE").val()) {
                doSave({});
            } else {
                bootbox.confirm("No ha seleccionado un centro de costo padre, ¿Quiere continuar así?", function (result) {
                    if (result) {
                        doSave({});
                    }
                });
            }
            
        }
    });
}

function overrideLoadOne(codCia, codCentroCosto) {
    const newURL = API.getOneURL
        .replaceAll('{codCia}', codCia)
        .replaceAll('{codCentroCosto}', codCentroCosto);

    loadOne({
        url: newURL,
        success: function (data) {
            if(data.success){ setDataToForm(data.data); }
        }
    });
}

function setDataToForm(data) {
    if (!isDefined(data)) return;

    if (isDefined(data.CENTRO_COSTO)) $('#CENTRO_COSTO').val(data.CENTRO_COSTO);
    if (isDefined(data.DESCRIPCION)) $('#DESCRIPCION').val(data.DESCRIPCION);
    if (isDefined(data.ACEPTA_DATOS) && data.ACEPTA_DATOS === 'S') { $('#ACEPTA_DATOS').prop('checked', true) }
}
