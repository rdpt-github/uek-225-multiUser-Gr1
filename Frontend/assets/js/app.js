﻿function rollDice(number) {
  const dice = [...document.querySelectorAll(".die-list")];
  counter = 0;
  dice.forEach(die => {
    toggleClasses(die);
    die.dataset.roll = number[counter];
    counter++;
    // die.dataset.roll = 1;
  });
}

function toggleClasses(die) {
  die.classList.toggle("odd-roll");
  die.classList.toggle("even-roll");
}

function getRandomNumber(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

document.getElementById("roll-button").addEventListener("click", rollDice);
