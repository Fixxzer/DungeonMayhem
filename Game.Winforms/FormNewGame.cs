using DungeonMayhem.Library;

namespace Game.Winforms
{
    public partial class FormNewGame : Form
    {
        public List<Creature> Creatures { get; set; }
        public bool UseMightyPowers { get; set; }

        private int _creatureId = 1;

        public FormNewGame()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            InitializeCharacters();
            this.UseMightyPowers = checkBoxUseMightyPowers.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeCharacters()
        {
            Creatures = new List<Creature>();

            // Azzan
            if (radioButtonAzzanComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Azzan", "Azzan.json"));
            }

            if (radioButtonAzzanHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Azzan", "Azzan.json", true));
            }

            // Blorp
            if (radioButtonBlorpComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Blorp", "Blorp.json"));
            }

            if (radioButtonBlorpHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Blorp", "Blorp.json", true));
            }

            // Delilah Deathray
            if (radioButtonDelilahComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Delilah Deathray", "DelilahDeathray.json"));
            }

            if (radioButtonDelilahHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Delilah Deathray", "DelilahDeathray.json", true));
            }

            // Dr. Tentaculous
            if (radioButtonDrTentaculousComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Dr. Tentaculous", "DrTentaculous.json"));
            }

            if (radioButtonDrTentaculousHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Dr. Tentaculous", "DrTentaculous.json", true));
            }

            // Hoots McGoots
            if (radioButtonHootsComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Hoots McGoots", "HootsMcGoots.json"));
            }

            if (radioButtonHootsHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Hoots McGoots", "HootsMcGoots.json", true));
            }

            // Jaheira
            if (radioButtonJaheiraComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Jaheira", "Jaheira.json"));
            }

            if (radioButtonJaheiraHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Jaheira", "Jaheira.json", true));
            }

            // Lia
            if (radioButtonLiaComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Lia", "Lia.json"));
            }

            if (radioButtonLiaHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Lia", "Lia.json", true));
            }

            // Lord Cinderpuff
            if (radioButtonLordCinderpuffComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Lord Cinderpuff", "LordCinderpuff.json"));
            }

            if (radioButtonLordCinderpuffHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Lord Cinderpuff", "LordCinderpuff.json", true));
            }

            // Mimi LeChaise
            if (radioButtonMimiComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Mimi LeChaise", "MimiLeChaise.json"));
            }

            if (radioButtonMimiHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Mimi LeChaise", "MimiLeChaise.json", true));
            }

            // Minsc & Boo
            if (radioButtonMinscComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Minsc & Boo", "MinscAndBoo.json"));
            }

            if (radioButtonMinscHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Minsc & Boo", "MinscAndBoo.json", true));
            }

            // Oriax
            if (radioButtonOriaxComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Oriax", "Oriax.json"));
            }

            if (radioButtonOriaxHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Oriax", "Oriax.json", true));
            }

            // Sutha
            if (radioButtonSuthaComputer.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Sutha", "Sutha.json"));
            }

            if (radioButtonSuthaHuman.Checked)
            {
                Creatures.Add(new Creature(_creatureId++, "Sutha", "Sutha.json", true));
            }
        }
    }
}
