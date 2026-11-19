# Kinds of object in the world

A reference for deciding what else could get a cube in future. Every thing the game keeps in its object
table has an **object kind** (Dalamud `ObjectKind`, FFXIVClientStructs `ObjectKind`). Battle NPCs also have a
**sub-kind** (`BattleNpcSubKind`). Values are the game's own names; the kind names below match both libraries
(Dalamud calls two of them `Area` and `Housing`, FFXIVClientStructs calls them `AreaObject` and
`HousingEventObject`).

Source: the enum names and the sub-kind descriptions come from the FFXIVClientStructs doc comments shipped
with Dalamud; the in-game examples and the "draws today" column come from the plugin's own code
(`Cube/Game/PlayerReader.cs`) and `docs/research-notes.md`. Kinds marked *(unverified)* have not been seen by
the plugin's probes.

## Object kinds

| Kind | What it is | In-game example | Drawn today? |
| --- | --- | --- | --- |
| `None` | An empty slot. | Not a real object. | No. |
| `Pc` | A player character, including you. | Other players in a city, party members. | Yes: player cube (you, party, alliance, other; colour by relation). |
| `BattleNpc` | An NPC that can fight, see sub-kinds below. | A monster in a FATE, a dungeon enemy, a summoner's pet, a Trust ally. | Yes, if "NPCs" is on and it has a name: NPC cube, enemy colour when hostile, `[T]` when targeted, `[!]` when it is attacking you. Pets and companions (sub-kinds `Pet`, `Buddy`) and anything owned by a player are always skipped. |
| `EventNpc` | A non-fighting NPC you talk to. | A vendor, a quest giver, an aetheryte attendant. | Yes, if "NPCs" is on and it has a name: NPC cube (purple). |
| `Treasure` | A treasure chest. | A dungeon chest, a treasure map chest. | No. |
| `Aetheryte` | An aetheryte crystal or shard. | The city aetheryte plaza crystal, a small shard in the field. | No. |
| `GatheringPoint` | A node you can gather from. | A mining or botanist node, a fishing spot marker. | No. |
| `EventObj` | An interactable object with no AI. | A door, a lever, a quest object, a bell, a housing ward sign, a hidden trigger. | Partly: only ones with a name show as text-labelled interactables, if "Objects" is on. Nameless ones are invisible triggers and icon-only markers and are skipped. |
| `MountType` | A mount that is in the world with no rider. | A mount you have summoned. *(unverified as a table entry)* | No. |
| `Companion` | A minion (glamour pet). | Your minion walking beside you. | No. |
| `Retainer` | A retainer standing in a summoning bell area. | Your retainer at the bell. | No. |
| `Area` (`AreaObject`) | An invisible area marker. | Zone or event area triggers, aoe-style markers the game places as objects. | No. |
| `Housing` (`HousingEventObject`) | A furnishing or exterior piece in a house or ward. | A chair, a fence, a house placard, a garden plot. | No. (Housing items with a name may also be `EventObj`, see above.) |
| `Cutscene` | An actor that exists only for a cutscene. | The characters in an in-engine cutscene. | No. |
| `CardStand` | A Triple Triad card stand. *(unverified)* | A Triple Triad table. | No. |
| `Ornament` | A fashion accessory worn on the back. | An ornament summoned by a player. | No. |

## Battle NPC sub-kinds

| Sub-kind | What it is | In-game example | Drawn today? |
| --- | --- | --- | --- |
| `Combatant` | An ordinary enemy or guard. | A monster in a FATE, a guard in a city. | Yes, as an NPC cube (enemy colour when hostile). |
| `Pet` | A pet owned by a player. | A summoner's carbuncle, a machinist's turret, a white mage's Lilybell. | No: owned pets are skipped. |
| `Buddy` | A Chocobo companion. | Your companion chocobo. | No: skipped like pets. |
| `Player` | A player standing in as a battle NPC. | Not seen as a table entry by the plugin. | Treated as an ordinary named NPC if it has a name and no owner. |
| `NpcPartyMember` | An NPC fighting on your side. | A Squadron member, a Trust, Duty Support. | Yes, as an NPC cube. |
| `BNpcPart` | A separate hit-able part of a bigger enemy. | A boss weak spot. | Drawn only if it has a name and no owner, which is rare. |
| `RaceChocobo` | A chocobo in Chocobo Racing. | Chocobo Racing at the Gold Saucer. | Treated as an ordinary named NPC if it has a name. |
| `LovmMinion` | A minion in Lord of Verminion. | The minions on the Verminion board. | Treated as an ordinary named NPC if it has a name. |

## Notes for later

- The plugin reads only the first slots of the object table (the character slots, plus a wider range when
  NPCs or objects are on), so something far down the table is never seen at all.
- A city's object table holds hundreds of entries, mostly `EventNpc` and `EventObj`; see "Named NPCs in the
  object table (#73)" and "Text-labelled interactable objects (#112)" in `docs/research-notes.md` for the real
  numbers and the filters chosen.
