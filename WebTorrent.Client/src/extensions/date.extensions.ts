interface Date {
    timeNow(): string;
    printDay(): string;
    timeNearest15(): string;
}

// Implement the timeNow method
Date.prototype.timeNow = function (): string {
    return ((this.getHours() < 10) ? "0" : "") + this.getHours() + ":" +
           ((this.getMinutes() < 10) ? "0" : "") + this.getMinutes();
};

Date.prototype.printDay = function () { 
    return ((this.getDate() < 10)?"0":"") + this.getDate() +"/"+(((this.getMonth()+1) < 10)?"0":"") + (this.getMonth()+1) +"/"+ this.getFullYear();
}

Date.prototype.timeNearest15 = function (): string {
    const minutes = this.getMinutes();
    const roundedMinutes = Math.round(minutes / 15) * 15;

    // Adjust the hours if rounding moves to the next hour
    let hours = this.getHours();
    if (roundedMinutes === 60) {
        hours = (hours + 1) % 24; // Wrap around if it's the next day
    }

    // Format hours and minutes to ensure two digits
    const formattedHours = hours < 10 ? "0" + hours : hours;
    const formattedMinutes = roundedMinutes % 60 < 10 ? "0" + (roundedMinutes % 60) : roundedMinutes % 60;

    return `${formattedHours}:${formattedMinutes}:00`;
};