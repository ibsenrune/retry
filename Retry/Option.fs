module OptionOperators

    let (>=>) f g =
        f >> Option.bind g

