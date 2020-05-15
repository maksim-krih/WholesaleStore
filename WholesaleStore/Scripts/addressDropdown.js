var regionUrl = '/Address/GetRegions';
var cityUrl = '/Address/GetCities';
var countryId = $('#CountryId');
var regionId = $('#RegionId');
var cityId = $('#CityId');

$('#CountryId').change(function () {
    regionId.empty();
    cityId.empty();
    $.getJSON(regionUrl, { countryId: $(this).val() }, function (data) {
        if (!data) {
            return;
        }
        //localities.append($('<option></option>').val('').text('Please select'));
        $.each(data, function (index, item) {
            regionId.append($('<option></option>').val(item.Value).text(item.Text));
        });

        if (data.length === 1) {
            $.getJSON(cityUrl, { regionId: data[0].Value }, function (data) {
                if (!data) {
                    return;
                }
                //localities.append($('<option></option>').val('').text('Please select'));
                $.each(data, function (index, item) {
                    cityId.append($('<option></option>').val(item.Value).text(item.Text));
                });
            });
        } 
    });
})

$('#RegionId').change(function () {
    cityId.empty();
    $.getJSON(cityUrl, { regionId: $(this).val() }, function (data) {
        if (!data) {
            return;
        }
        //localities.append($('<option></option>').val('').text('Please select'));
        $.each(data, function (index, item) {
            cityId.append($('<option></option>').val(item.Value).text(item.Text));
        });
    });
})