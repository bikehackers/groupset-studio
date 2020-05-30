namespace GroupSetStudio

type Side =
  | Left
  | Right

type Handedness =
  | Specific of Side
  | Ambi

type Shifter =
  {
    Manufacturer : string
    ProductCode : string
    Speed : int
    CablePull : float
    Side : Handedness
  }

type RearDerailleur =
  {
    Manufacturer : string
    ProductCode : string
    ActuationRatio : int * int
    Speed : int
    Weight : int
    LargestSprocketMaxTeeth : int
    LargestSprocketMinTeeth : int
    SmallestSprocketMaxTeeth : int
    SmallestSprocketMinTeeth : int
    Capacity : int
    Clutched : bool
  }

type Cassette =
  {
    Manufacturer : string
    ProductCode : string
    Sprockets : int list
    SprocketPitch : float
    Interface : string
  }

type Chain =
  {
    Manufacturer : string
    ProductCode : string
    Speed : int
    Weight : int
  }
