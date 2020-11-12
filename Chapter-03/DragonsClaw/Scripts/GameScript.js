$(document).ready(function () {
    $('a.nav-link').click(function (e) {
        e.preventDefault();
    });
});

function toggleAccountTab() {
    $('#account-details').toggle();
}

/* Empire */
function empireHide() {
    $('#empire-content').slideUp();
}

function empireShow() {
    $.ajax({
        type: 'GET',
        url: '/Empire/Index',
        data: '',
        contentType: 'application/json',
        dataType: 'html',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });

    fillPlayerHeader();
}

function createEmpire() {
    $('#empire-content').slideUp();

    $.ajax({
        type: 'GET',
        url: '/Empire/Create',
        data: '',
        contentType: 'application/json',
        dataType: 'html',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });
}

function doCreateEmpire() {
    var data = $('#create-empire-form').serialize();

    $('#empire-content').slideUp();

    $.ajax({
        type: 'POST',
        url: '/Empire/Create',
        data: data,
        dataType: 'json',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });
}

/* Player */
function fillPlayerHeader() {
}

function showEPanel1() {
    $('#epanel1').show();
    $('#epanel2').hide();
}

function showEPanel2() {
    $('#epanel1').hide();
    $('#epanel2').show();
}

/* Milatary */

function militaryShow() {
    $.ajax({
        type: 'GET',
        url: '/Game/Military',
        data: '',
        contentType: 'application/json',
        dataType: 'html',
        success: function (response) {
            $('#military-content').html(response);
            $('#military-content').slideDown();
        }
    });

    fillPlayerHeader();
}

function militaryHide() {
    $('#military-content').slideUp();
}
// Troop Training area
function fillOffense(value) {
    $('#OffensiveUnits').val(value);
}

function fillDefense(value) {
    $('#DefensiveUnits').val(value);
}

function fillSpecialty(value) {
    $('#SpecialtyUnits').val(value);
}

function fillSpies(value) {
    $('#SpyUnits').val(value);
}

function fillFighters(value) {
    $('#FighterUnits').val(value);
}

function fillBombers(value) {
    $('#BomberUnits').val(value);
}

function fillCruisers(value) {
    $('#CruiserUnits').val(value);
}

function fillDestroyers(value) {
    $('#DestroyerUnits').val(value);
}

function fillDreadnaughts(value) {
    $('#DreadnaughtUnits').val(value);
}

function trainTroops() {

    var data = $('#training-form').serialize();

    $.ajax({
        type: 'POST',
        url: '/Game/Military',
        data: data,
        dataType: 'json',
        success: function (response) {
            $('#military-content').html(response);
        },
        error: function (response) {
            $('#military-content').html(response.responseText);
        },
        complete: function (data) {
            if (data.status === 200) {
                $('#military-content').html(data.responseText);
            }
        }
    });

    fillPlayerHeader();
}
