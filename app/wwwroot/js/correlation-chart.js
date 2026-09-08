(function () {
    const dataElement = document.getElementById("correlation-data");
    const canvas = document.getElementById("correlation-chart");

    if (!dataElement || !canvas) {
        return;
    }

    const rows = JSON.parse(dataElement.textContent);
    const context = canvas.getContext("2d");
    const colors = {
        sensor: "#0d6efd",
        weather: "#198754",
        axis: "#6c757d",
        grid: "#dee2e6",
        text: "#212529"
    };

    function drawChart() {
        const width = canvas.clientWidth;
        const height = canvas.clientHeight;
        const ratio = window.devicePixelRatio || 1;
        const padding = { top: 24, right: 24, bottom: 42, left: 52 };

        canvas.width = width * ratio;
        canvas.height = height * ratio;
        context.setTransform(ratio, 0, 0, ratio, 0, 0);
        context.clearRect(0, 0, width, height);

        const values = rows.flatMap(row => [row.SensorValue, row.WeatherValue])
            .filter(value => value !== null && value !== undefined);

        if (values.length === 0) {
            context.fillStyle = colors.axis;
            context.font = "14px system-ui, sans-serif";
            context.textAlign = "center";
            context.fillText("Brak wartości do narysowania", width / 2, height / 2);
            return;
        }

        const minimum = Math.min(...values);
        const maximum = Math.max(...values);
        const range = maximum === minimum ? 1 : maximum - minimum;
        const chartWidth = width - padding.left - padding.right;
        const chartHeight = height - padding.top - padding.bottom;
        const x = index => padding.left + (rows.length === 1 ? chartWidth / 2 : index * chartWidth / (rows.length - 1));
        const y = value => padding.top + (maximum - value) * chartHeight / range;

        context.strokeStyle = colors.grid;
        context.lineWidth = 1;
        context.font = "12px system-ui, sans-serif";
        context.fillStyle = colors.axis;
        context.textAlign = "right";

        for (let step = 0; step <= 4; step += 1) {
            const value = minimum + range * (1 - step / 4);
            const lineY = padding.top + step * chartHeight / 4;
            context.beginPath();
            context.moveTo(padding.left, lineY);
            context.lineTo(width - padding.right, lineY);
            context.stroke();
            context.fillText(value.toFixed(1), padding.left - 8, lineY + 4);
        }

        context.strokeStyle = colors.axis;
        context.beginPath();
        context.moveTo(padding.left, padding.top);
        context.lineTo(padding.left, height - padding.bottom);
        context.lineTo(width - padding.right, height - padding.bottom);
        context.stroke();

        drawSeries("SensorValue", colors.sensor);
        drawSeries("WeatherValue", colors.weather);

        function drawSeries(property, color) {
            context.strokeStyle = color;
            context.lineWidth = 2;
            context.beginPath();
            let drawing = false;

            rows.forEach((row, index) => {
                const value = row[property];
                if (value === null || value === undefined) {
                    drawing = false;
                    return;
                }

                const pointX = x(index);
                const pointY = y(value);
                if (!drawing) {
                    context.moveTo(pointX, pointY);
                    drawing = true;
                } else {
                    context.lineTo(pointX, pointY);
                }
            });

            context.stroke();
            context.fillStyle = color;
            rows.forEach((row, index) => {
                const value = row[property];
                if (value === null || value === undefined) {
                    return;
                }

                context.beginPath();
                context.arc(x(index), y(value), 3, 0, Math.PI * 2);
                context.fill();
            });
        }
    }

    new ResizeObserver(drawChart).observe(canvas);
    drawChart();
})();