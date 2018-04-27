namespace Okta.AspNet.Abstractions
{
    public class OktaWebApiOptionsValidator : OktaOptionsValidator
    {
        public void Validate(OktaWebApiOptions options)
        {
            base.ValidateBaseOktaOptions(options);
        }
    }
}
